using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceProcess;
using TaskScheduler;
using System.Diagnostics;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace NightBuilder
{
    /// <summary>
    /// Результат выполнения действия.
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Признак успешности выполнения действия.
        /// </summary>
        private bool isEnd = false;
        /// <summary>
        /// Сообщение об одиночной ошибке.
        /// </summary>
        private string error = "";
        /// <summary>
        /// Список ошибок.
        /// </summary>
        private ArrayList errorList;
        /// <summary>
        /// Признак успешности выполнения действия.
        /// </summary>
        public bool IsEnd
        {
            get
            {
                return isEnd;
            }
        }
        /// <summary>
        /// Сообщение об одиночной ошибке.
        /// </summary>
        public string Error
        {
            get
            {
                return error;
            }
        }
        /// <summary>
        /// Список ошибок.
        /// </summary>
        public ArrayList ErrorList
        {
            get
            {
                return errorList;
            }
        }

        public ActionResult() { }

        public ActionResult(bool isEnd, string error, ArrayList errorList = null) 
        {
            this.isEnd = isEnd;
            this.error = error;
            this.errorList = errorList;
        }
    }

    /// <summary>
    /// Класс выполнения действий.
    /// </summary>
    public static class RealAction
    {     
        /// <summary>
        /// Создать резервную копию файла.
        /// </summary>
        /// <param name="fileName"> полный путь к файлу, который копируется </param>
        /// <param name="backupFolder"> полный путь к папке для резервной копии </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult BackupFile(string fileName, string backupFolder)
        {
            try
            {
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }
            }
            catch (Exception e)
            {
                return new ActionResult(false, e.Message);
            }
            return CopyFile(fileName, backupFolder);
        }

        /// <summary>
        /// Скопировать файл.
        /// </summary>
        /// <param name="fileName"> полный путь к файлу, который копируется </param>
        /// <param name="destinationFolder"> полный путь к папке, куда копируется </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult CopyFile(string fileName, string destinationFolder)
        {
            try
            {
                FileInfo file = new FileInfo(fileName);
                string copyFileName = Path.Combine(destinationFolder, file.Name);
                File.Copy(fileName, copyFileName, true);
                file = new FileInfo(copyFileName);
                file.IsReadOnly = false;
            }
            catch (Exception e)
            {
                return new ActionResult(false, e.Message);
            }
            return new ActionResult(true, "");
        }

        /// <summary>
        /// Создать резервную копию папки.
        /// </summary>
        /// <param name="folderName"> полный путь к папке, которая копируется </param>
        /// <param name="backupFolder"> полный путь к папке, куда копировать </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult BackupFolder(string folderName, string backupFolder, Operation.ErrorTypes errorType)
        {
            if (!Directory.Exists(folderName))
            {
                return new ActionResult(false, "Folder '" + folderName + "' not found.");
            }
            try
            {
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }
            }
            catch (Exception e)
            {
                return new ActionResult(false, e.Message);
            }
            return CopyFolder(folderName, backupFolder, errorType);
        }

        /// <summary>
        /// Копировать папку.
        /// </summary>
        /// <param name="folderName"> полный путь к папке, которая копируется </param>
        /// <param name="destinationFolder"> полный путь к папке, куда копировать </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult CopyFolder(string folderName, string destinationFolder, Operation.ErrorTypes errorType)
        {
            bool isSuccessful = false;
            string msg = "";
            ArrayList errorList = new ArrayList();
            try
            {
                DirectoryInfo mainDir = new DirectoryInfo(folderName);

                string folder = Path.Combine(destinationFolder, mainDir.Name);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                foreach (FileInfo file in mainDir.GetFiles("*.*"))
                {
                    try
                    {
                        string copyFileName = Path.Combine(folder, file.Name);
                        File.Copy(file.FullName, copyFileName, true);
                        FileInfo fi = new FileInfo(copyFileName);
                        fi.IsReadOnly = false;
                    }
                    catch (Exception e)
                    {
                        if (errorType == Operation.ErrorTypes.STOP_ON_ERROR)
                        {
                            return new ActionResult(false, e.Message);
                        }
                        errorList.Add(e.Message);
                    }
                }
                foreach (DirectoryInfo dir in mainDir.GetDirectories())
                {
                    ActionResult result = CopyFolder(dir.FullName, folder, errorType);
                    if (!result.IsEnd)
                    {
                        if (errorType == Operation.ErrorTypes.STOP_ON_ERROR)
                        {
                            return new ActionResult(false, result.Error);
                        }
                        AddResultToErrorList(errorList, result); 
                    } 
                }
                if (errorList.Count == 0)
                {
                    isSuccessful = true;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new ActionResult(isSuccessful, msg, errorList);
        }

        /// <summary>
        /// Проверить подключение к БД.
        /// </summary>
        /// <param name="sqlConnect"> строка подключения </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult ConnectToDB(string sqlConnect)
        {
            string msg = "";
            bool isSuccessful = false;
            try
            {
                SqlConnection connection = new SqlConnection(sqlConnect);
                connection.Open();
                connection.Close();
                isSuccessful = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Создать резервную копию БД.
        /// </summary>
        /// <param name="dbName"> название БД </param>
        /// <param name="backupFolderName"> папка, где будет размещена резервная копия </param>
        /// <param name="sqlConnect"> строка подключения к БД </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult BackupDB(string dbName, string backupFolderName, string sqlConnect)
        {
            string msg = "";
            bool isSuccessful = false;
            try
            {
                if (!Directory.Exists(backupFolderName))
                {
                    Directory.CreateDirectory(backupFolderName);
                }
                string filePath = Path.Combine(backupFolderName, string.Format("{0}-{1}.bak", dbName, DateTime.Now.ToString("yyyy-MM-dd")));
                SqlConnection con = new SqlConnection(sqlConnect);
                Server server = new Server(new ServerConnection(con));
                Backup source = new Backup();
                source.CompressionOption = BackupCompressionOptions.On;
                source.Action = BackupActionType.Database;
                source.Database = dbName;
                BackupDeviceItem destination = new BackupDeviceItem(filePath, DeviceType.File);
                source.Devices.Add(destination);
                source.SqlBackup(server);
                isSuccessful = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Запустить SQL-скрипт.
        /// </summary>
        /// <param name="queryFile"> файл SQL-скрипта </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult RunSQLQuery(string queryFile, string sqlConnect)
        {
            string msg = "";
            bool isSuccessful = false;
            try
            {
                string script = File.ReadAllText(queryFile, Encoding.Default);
                SqlConnection conn = new SqlConnection(sqlConnect);
                Server server = new Server(new ServerConnection(conn));
                server.ConnectionContext.ExecuteNonQuery(script);
                isSuccessful = true;
            }
            catch (Exception e)
            {
                msg = queryFile + " " + e.Message;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Запустить все SQL-скрипты в папке.
        /// </summary>
        /// <param name="sqlFolder"> папка с SQL-скриптами </param>
        /// <param name="sqlConnect"> строка подключения к БД </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult RunSQLQueryInFolder(string sqlFolder, string sqlConnect, Operation.ErrorTypes errorType)
        {
            string msg = "";
            bool isSuccessful = false;
            ArrayList errorList = new ArrayList();
            try
            {
                DirectoryInfo mainDir = new DirectoryInfo(sqlFolder);

                foreach (FileInfo file in mainDir.GetFiles("*.sql"))
                {
                    ActionResult result = RunSQLQuery(file.FullName, sqlConnect);
                    if (!result.IsEnd)
                    {
                        if (errorType == Operation.ErrorTypes.STOP_ON_ERROR)
                        {
                            return new ActionResult(false, result.Error);
                        }
                        AddResultToErrorList(errorList, result); 
                    }
                }
                foreach (DirectoryInfo dir in mainDir.GetDirectories())
                {
                    ActionResult result = RunSQLQueryInFolder(dir.FullName, sqlConnect, errorType);
                    if (!result.IsEnd)
                    {
                        if (errorType == Operation.ErrorTypes.STOP_ON_ERROR)
                        {
                            return new ActionResult(false, result.Error);
                        }
                        AddResultToErrorList(errorList, result);
                    }
                }
                if (errorList.Count == 0)
                {
                    isSuccessful = true;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new ActionResult(isSuccessful, msg, errorList);
        }

        /// <summary>
        /// Изменить состояние службы.
        /// </summary>
        /// <param name="serviceName"> краткое название службы </param>
        /// <param name="isStart"> запустить, если true, остановить, если false </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult ChangeServiceState(string serviceName, bool isStart)
        {
            string msg = "";
            bool isSuccessful = false;
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (isStart)
                {
                    service.Start();
                }
                else
                {
                    service.Stop();
                }
                isSuccessful = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Включить службу.
        /// </summary>
        /// <param name="serviceName"> название службы </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult RunService(string serviceName)
        {
            return ChangeServiceState(serviceName, true);
        }

        /// <summary>
        /// Выключить службу.
        /// </summary>
        /// <param name="serviceName"> название службы </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult StopService(string serviceName)
        {
            return ChangeServiceState(serviceName, false);
        }

        /// <summary>
        /// Изменить состояние задачи в Windows Task Scheduler.
        /// </summary>
        /// <param name="taskName"> название задачи </param>
        /// <param name="isStart"> запустить, если true, остановить, если false </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult ChangeTaskState(string taskName, bool isStart)
        {
            string msg = "";
            bool isSuccessful = false;
            try
            {
                ITaskService ts = new TaskSchedulerClass();
                ts.Connect("localhost");
                ITaskFolder taskFolder = ts.GetFolder("\\");
                IRegisteredTask task = taskFolder.GetTask(taskName);
                if (isStart)
                {
                    task.Run(null);
                }
                else
                {
                    task.Stop(0);
                    task.Enabled = false;
                }
                isSuccessful = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Запустить задачу в Планировщике заданий
        /// </summary>
        /// <param name="taskName"> название задачи </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult RunTask(string taskName)
        {
            return ChangeTaskState(taskName, true);
        }

        /// <summary>
        /// Остановить задачу в Планировщике заданий
        /// </summary>
        /// <param name="taskName"> название задачи </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult StopTask(string taskName)
        {
            return ChangeTaskState(taskName, false);
        }

        /// <summary>
        /// Запусть процесс.
        /// </summary>
        /// <param name="processName"> название процесса </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult RunProccess(string processName)
        {
            string msg = "";
            bool isSuccessful = false;
            try
            {
                Process.Start(processName);
                isSuccessful = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
                isSuccessful = false;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Остановить процесс.
        /// </summary>
        /// <param name="processName"> название процесса </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult StopProcess(string processName)
        {
            string msg = "";
            bool isSuccessful = false;
            try
            {
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                    isSuccessful = true;
                }
                if (!isSuccessful)
                {
                    msg = "Not found this process.";
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                isSuccessful = false;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="fileName"> полный путь и название файла </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult DeleteFile(string fileName)
        {
            string msg = "";
            bool isSuccessful = false;
            try
            {
                if (!File.Exists(fileName))
                {
                    return new ActionResult(false, "File '" + fileName + "' not found.");
                }
                File.Delete(fileName);
                isSuccessful = true;
            }
            catch (Exception e)
            {
                msg = e.Message;
                isSuccessful = false;
            }
            return new ActionResult(isSuccessful, msg);
        }

        /// <summary>
        /// Удалить папку.
        /// </summary>
        /// <param name="folderName"> полный путь к папке </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult DeleteFolder(string folderName, Operation.ErrorTypes errorType)
        {
            string msg = "";
            bool isSuccessful = false;
            ArrayList errorList = new ArrayList();
            try
            {
                DirectoryInfo mainDir = new DirectoryInfo(folderName);
                foreach (FileInfo file in mainDir.GetFiles("*.*"))
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch (Exception e)
                    {
                        if (errorType == Operation.ErrorTypes.STOP_ON_ERROR)
                        {
                            return new ActionResult(false, e.Message);
                        }
                        errorList.Add(e.Message);
                    }
                }
                foreach (DirectoryInfo dir in mainDir.GetDirectories())
                {
                    ActionResult result = DeleteFolder(dir.FullName, errorType);
                    if (!result.IsEnd)
                    {
                        if (errorType == Operation.ErrorTypes.STOP_ON_ERROR)
                        {
                            return new ActionResult(false, result.Error);
                        }
                        AddResultToErrorList(errorList, result); 
                    }
                }
                Directory.Delete(folderName, true);
                if (errorList.Count == 0)
                {
                    isSuccessful = true;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                isSuccessful = false;
            }
            return new ActionResult(isSuccessful, msg, errorList);
        }

        /// <summary>
        /// Добавить результат выполнения действия в список ошибок.
        /// </summary>
        /// <param name="errorList"> список ошибок </param>
        /// <param name="result"> добавляемый результат действия </param>
        private static void AddResultToErrorList(ArrayList errorList, ActionResult result)
        {
            if (result.ErrorList == null)
            {
                errorList.Add(result.Error);
            }
            else
            {
                errorList.AddRange(result.ErrorList);
            }
        }

        /// <summary>
        /// Генерировать папку.
        /// </summary>
        /// <param name="backupFolder"> путь к папке </param>
        /// <returns> результат выполнения </returns>
        public static ActionResult GenerateFolder(string backupFolder)
        {
            try
            {
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }
            }
            catch (Exception e)
            {
                return new ActionResult(false, e.Message);
            }
            return new ActionResult(true, "");
        }
    }
}

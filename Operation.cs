﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace NightBuilder
{
    /// <summary>
    /// Обёртка для пары <Тип перечисления, строка>
    /// </summary>
    /// <typeparam name="T"> тип перечисления </typeparam>
    public class EnumWrapper<T>
    {
        private KeyValuePair<T, string> pair;

        public EnumWrapper(T type, string value)
        {
            this.pair = new KeyValuePair<T, string>(type, value);
        }

        public override string ToString()
        {
            return pair.Value;
        }

        public T GetConst()
        {
            return pair.Key;
        }
    }

    [Serializable]
    public class Operation
    {
        /// <summary>
        /// Тип операции
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public OperationTypes type = OperationTypes.EMPTY;
        /// <summary>
        /// Источник ресурса для операции (файл, задачи и пр.)
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public string sourceName = "";
        /// <summary>
        /// Цель операции (например, куда копировать)
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public string destinationName = "";
        /// <summary>
        /// Тип обработки ошибки, возникшей во время операции
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public ErrorTypes errorType = ErrorTypes.EMPTY;
        /// <summary>
        /// Признак, что операцию нужно выполнить с админскими правами (зарезервированный признак)
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public bool isAdmin = true;
        /// <summary>
        /// Адрес SQL-сервера
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public string sqlServer = "";
        /// <summary>
        /// Логин для SQL-сервера
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public string dbLogin = "";
        /// <summary>
        /// Пароль для SQL-сервера
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public string dbPassword = "";
        /// <summary>
        /// Логин для SQL-сервера
        /// </summary>
        //[System.Xml.Serialization.XmlElementAttribute()]
        //public string operationName = "";
        /// <summary>
        /// Номер родительской задачи
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public int parentIndex = -1;
        /// <summary>
        /// Признак, что нужно генерировать папку для резервной копии
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute()]
        public bool isGenerateBackupFolder = false;

        /// <summary>
        /// Папка для резервной копии
        /// </summary>
        private string backupFolder = "";
        /// <summary>
        /// Родительская операция
        /// </summary>
        private Operation parentOperation;
        /// <summary>
        /// Смещение для кодирования пароля
        /// </summary>
        const char CODE_DELTA = 'A';

        public Operation() { }

        public Operation(OperationTypes type, string sourceName, ErrorTypes errorType, bool isAdmin, string destinationName, string sqlServer, string dbLogin, string dbPassword, int parentIndex, bool isGenerateBackupFolder)
        {
            this.type = type;
            this.sourceName = sourceName;
            this.errorType = errorType;
            this.isAdmin = isAdmin;
            this.destinationName = destinationName;
            this.sqlServer = sqlServer;
            this.dbLogin = dbLogin;
            this.dbPassword = CodePassword(dbPassword);
            this.parentIndex = parentIndex;
            this.isGenerateBackupFolder = isGenerateBackupFolder;
            if (type == OperationTypes.GEN_FOLDER)
            {
                this.isGenerateBackupFolder = true;
            }
            SetBackupFolder();
        }

        public Operation(Operation operation)
        {
            this.type = operation.type;
            this.sourceName = operation.sourceName;
            this.errorType = operation.errorType;
            this.isAdmin = operation.isAdmin;
            this.destinationName = operation.destinationName;
            this.sqlServer = operation.sqlServer;
            this.dbLogin = operation.dbLogin;
            this.dbPassword = operation.dbPassword;
            this.parentIndex = operation.parentIndex;
            this.parentOperation = operation.parentOperation;
            this.backupFolder = operation.backupFolder;
            this.isGenerateBackupFolder = operation.isGenerateBackupFolder;
        }

        /// <summary>
        /// Кодировать пароль.
        /// </summary>
        /// <param name="password"> пароль </param>
        /// <returns> закодированный пароль </returns>
        private static string CodePassword(string password)
        {
            char[] charArray = password.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                charArray[i] += CODE_DELTA;
            }
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// Декодировать пароль.
        /// </summary>
        /// <param name="password"> пароль </param>
        /// <returns> декодированный пароль </returns>
        private static string DecodePassword(string password)
        {
            char[] charArray = password.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                charArray[i] -= CODE_DELTA;
            }
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// Задать каталог для резервной копии.
        /// </summary>
        public void SetBackupFolder()
        {
            bool isGenerate = isGenerateBackupFolder;
            string destination = destinationName;
            if (parentOperation != null)
            {
                isGenerate = parentOperation.isGenerateBackupFolder;
                destination = parentOperation.destinationName;
            }
            if (isGenerate)
            {
                backupFolder = GenerateFolder(destination);
            }
            else
            {
                backupFolder = destination;
            }
        }

        /// <summary>
        /// Сгенерировать название папки.
        /// </summary>
        /// <param name="forder"> папка для резервной копии </param>
        /// <returns> сгенерированная папка </returns>
        private static string GenerateFolder(string forder)
        {
            return forder + String.Format("\\{0:yyyy-MM-dd-hh-mm-ss}", DateTime.Now);
        }

        /// <summary>
        /// Задать текущей операции родительскую операцию.
        /// </summary>
        /// <param name="operation"> родительская операция </param>
        public void SetParent(Operation operation)
        {
            this.parentOperation = operation;
            if (operation == null)
            {
                this.backupFolder = "";
            }
            else
            {
                this.backupFolder = operation.backupFolder;
            }
        }

        /// <summary>
        /// Перечисление типов операций.
        /// </summary>
        public enum OperationTypes 
        {
            EMPTY = -1,
            BACKUP_DB = 0, 
            BACKUP_FILE = 1, 
            BACKUP_FOLDER = 2,
            CONNECT_DB = 3,
            COPY_FILE = 4,
            COPY_FOLDER = 5,
            DELETE_FILE = 6,
            DELETE_FOLDER = 7,
            RUN_PROCESS = 8,
            RUN_SERVICE = 9,
            RUN_SQL_QUERIES_IN_FOLDER = 10,
            RUN_SQL_QUERY = 11,
            RUN_TASK = 12,
            STOP_PROCESS = 13,
            STOP_SERVICE = 14,
            STOP_TASK = 15,
            STOP_SCENARIO = 16,
            GEN_FOLDER = 17,
            CREATE_VAR = 18//,
            //COPY_FILES_IN_FOLDER = 19,
            //BACKUP_FILES_IN_FOLDER = 20
        }

        /// <summary>
        /// Строковые названия типов операций.
        /// </summary>
        private static string[] operTypes = {"Backup DB", "Backup file", "Backup folder", "Connect to DB", "Copy file", "Copy folder",
                                             "Delete file", "Delete folder",
                                             "Run process", "Run service", "Run SQL-queries in folder", "Run SQL-query", "Run task", 
                                             "Stop process",  "Stop service", "Stop task", "Stop scenario",
                                             "Generate folder", "Create variable"};//, "Copy files in folder", "Backup files in folder"};

        /// <summary>
        /// Строковые названия типов реагирования на ошибки.
        /// </summary>
        private static string[] errTypes = {"Resume next", "Stop on error"};

        /// <summary>
        /// Перечисление типов реагирования на ошибки.
        /// </summary>
        public enum ErrorTypes
        {
            EMPTY = -1,
            RESUME_NEXT = 0,
            STOP_ON_ERROR = 1
        }

        /// <summary>
        /// Выдать список типов операций.
        /// </summary>
        /// <returns> список типов операций </returns>
        public static ArrayList GetOperationTypes()
        {
            ArrayList arr = new ArrayList();
            foreach (Operation.OperationTypes type in Enum.GetValues(typeof(Operation.OperationTypes)))
            {
                if (type.GetHashCode() >= 0)
                {
                    arr.Add(new EnumWrapper<OperationTypes>(type, Operation.ToString(type)));
                }
            }
            return arr;
        }

        /// <summary>
        /// Выдать список типов реагирования на ошибки.
        /// </summary>
        /// <returns> список типов реагирования на ошибки </returns>
        public static ArrayList GetErrorTypes()
        {
            ArrayList arr = new ArrayList();
            foreach (Operation.ErrorTypes type in Enum.GetValues(typeof(Operation.ErrorTypes)))
            {
                if (type.GetHashCode() >= 0)
                { 
                    arr.Add(new EnumWrapper<ErrorTypes>(type, Operation.ToString(type))); 
                }
            }
            return arr;
        }

        /// <summary>
        /// Перевести в строку тип реагирования на ошибку.
        /// </summary>
        /// <param name="type"> тип реагирования на ошибку </param>
        /// <returns> строковое представление </returns>
        public static string ToString(ErrorTypes type)
        {
            return errTypes[type.GetHashCode()];
        }

        /// <summary>
        /// Перевести в строку тип операции.
        /// </summary>
        /// <param name="type"> тип операции </param>
        /// <returns> строковое представление </returns>
        public static string ToString(OperationTypes type)
        {
            return operTypes[type.GetHashCode()];
        }

        /// <summary>
        /// Перевести в строку операцию.
        /// </summary>
        /// <returns> строковое представление </returns>
        public override string ToString()
        {
            switch (type)
            {
                case OperationTypes.BACKUP_DB:
                    return "DB name = '" + sourceName + "', dir = '" + destinationName + "'";
                case OperationTypes.BACKUP_FILE:
                case OperationTypes.COPY_FILE:
                    return "File = '" + sourceName + "', dir = '" + destinationName + "'";
                case OperationTypes.BACKUP_FOLDER:
                case OperationTypes.COPY_FOLDER:
                //case OperationTypes.COPY_FILES_IN_FOLDER:
                //case OperationTypes.BACKUP_FILES_IN_FOLDER:
                    return "Original dir = '" + sourceName + "', copy dir = '" + destinationName + "'";
                case OperationTypes.CONNECT_DB:
                    return "DB name = '" + sourceName + "', SQL server = '" + sqlServer + "', login = '" + dbLogin + "' password = '******'";
                case OperationTypes.RUN_SQL_QUERY:
                    return "File = '" + sourceName + "'";
                case OperationTypes.RUN_SQL_QUERIES_IN_FOLDER:
                    return "SQL dir = '" + sourceName + "'";
                case OperationTypes.GEN_FOLDER:
                    return "Dir = '" + destinationName + "'";
                case OperationTypes.CREATE_VAR:
                    return "Name = '" + sourceName + "', value = '" + destinationName + "'";
                default:
                    return "Name = '" + sourceName + "'";
            }
            //return base.ToString();
        }

        /// <summary>
        /// Выдать признак остановки при ошибке.
        /// </summary>
        /// <returns>Да/нет</returns>
        public bool IsStopOnError()
        {
            return errorType == ErrorTypes.STOP_ON_ERROR ? true : false;
        }

        /// <summary>
        /// Выдать строку подключения к БД.
        /// </summary>
        /// <returns> строка подключения </returns>
        private string GetConnectionString()
        {
            if (parentOperation == null)
            {
                return "Server=" + sqlServer + ";Database=" + sourceName + ";User Id=" + dbLogin + ";Password=" + DecodePassword(dbPassword) + ";";
            }
            return parentOperation.GetConnectionString();
        }

        /// <summary>
        /// Заменить имя переменной на её значение.
        /// </summary>
        /// <param name="str"> входная строка </param>
        /// <param name="variables"> словарь переменных </param>
        /// <returns> строка с заменёнными переменными </returns>
        private string ReplaceVariables(string str, Dictionary<string, string> variables)
        {
            int firstIndex = str.IndexOf('%', 0);
            int secondIndex = -1;
            if (firstIndex >= 0 && firstIndex < str.Length - 1)
            {
                secondIndex = str.IndexOf('%', firstIndex + 1);
            }
            if (firstIndex >= 0 && secondIndex >= 0)
            {
                string variable = str.Substring(firstIndex + 1, secondIndex - firstIndex - 1);
                return str.Replace("%" + variable.ToUpper() + "%", variables[variable]);
            }
            return str;
        }

        /// <summary>
        /// Выполнить операцию.
        /// </summary>
        /// <returns> результат выполнения с детализацией в случае ошибки </returns>
        public ActionResult Execute(Dictionary<string, string> variables)
        {
            string sourceNameCopy = ReplaceVariables(sourceName, variables);
            string destinationNameCopy = ReplaceVariables(destinationName, variables);
            string backupFolderCopy = ReplaceVariables(backupFolder, variables);
            switch (type)
            {
                case OperationTypes.BACKUP_DB:
                    if (parentOperation == null)
                    {
                        return new ActionResult(false, "Not found parent operation.");
                    }
                    return RealAction.BackupDB(sourceNameCopy, destinationNameCopy, GetConnectionString());
                case OperationTypes.BACKUP_FILE:
                    return RealAction.BackupFile(sourceNameCopy, backupFolderCopy);
                case OperationTypes.BACKUP_FOLDER:
                    return RealAction.BackupFolder(sourceNameCopy, backupFolderCopy, errorType);
                case OperationTypes.CONNECT_DB:
                    return RealAction.ConnectToDB(GetConnectionString());
                case OperationTypes.COPY_FILE:
                    return RealAction.CopyFile(sourceNameCopy, destinationNameCopy);
                case OperationTypes.COPY_FOLDER:
                    return RealAction.CopyFolder(sourceNameCopy, destinationNameCopy, errorType);
                case OperationTypes.RUN_PROCESS:
                    return RealAction.RunProccess(sourceNameCopy);
                case OperationTypes.RUN_SERVICE:
                    return RealAction.RunService(sourceNameCopy);
                case OperationTypes.RUN_SQL_QUERY:
                    return RealAction.RunSQLQuery(sourceNameCopy, GetConnectionString());
                case OperationTypes.RUN_TASK:
                    return RealAction.RunTask(sourceNameCopy);
                case OperationTypes.STOP_PROCESS:
                    return RealAction.StopProcess(sourceNameCopy);
                case OperationTypes.STOP_SERVICE:
                    return RealAction.StopService(sourceNameCopy);
                case OperationTypes.STOP_TASK:
                    return RealAction.StopTask(sourceNameCopy);
                case OperationTypes.RUN_SQL_QUERIES_IN_FOLDER:
                    return RealAction.RunSQLQueryInFolder(sourceNameCopy, GetConnectionString(), errorType);
                case OperationTypes.DELETE_FILE:
                    return RealAction.DeleteFile(sourceNameCopy);
                case OperationTypes.DELETE_FOLDER:
                    return RealAction.DeleteFolder(sourceNameCopy, errorType);
                case OperationTypes.GEN_FOLDER:
                    return RealAction.GenerateFolder(backupFolderCopy);
                case OperationTypes.STOP_SCENARIO:
                case OperationTypes.CREATE_VAR:
                    return new ActionResult(true, "");
                //case OperationTypes.COPY_FILES_IN_FOLDER:
                //case OperationTypes.BACKUP_FILES_IN_FOLDER:
                //    return new ActionResult(false, "Create events!!!!!");
            }
            return new ActionResult(false, "Execute unknown operation type."); ;
        }
    }
}

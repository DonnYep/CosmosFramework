using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos
{
    [CreateAssetMenu(fileName = "SQLDataSet", menuName = "CosmosFramework/SQLDataSet/MySQL")]
    public class SQLDataSet : CFDataSet
    {
        [SerializeField] string serverAddress;
        public string ServerAddress { get { return serverAddress; } set { serverAddress = value; } }
        [SerializeField] string port;
        public string Port { get { return port; } set { port = value; } }
        [SerializeField] string databaseName;
        public string DatabaseName { get { return databaseName; } set { databaseName = value; } }
        [SerializeField] string user;
        public string User { get { return user; } set { user = value; } }
        [SerializeField] string password;
        public string Password { get { return password; } set { password = value; } }
        public override void Reset()
        {
            objectName = "SQLConnectProfile";
            serverAddress = "localhost";
            port = "3306";
            databaseName = "JYGameDatabase";
            user = "root";
            password = "JYGamePWD";
        }
    }
}
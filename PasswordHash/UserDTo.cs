using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;
using K4os.Compression.LZ4.Streams.Adapters;
using System.Data;
using Mysqlx.Resultset;
using System.ComponentModel.Design;
using Google.Protobuf.WellKnownTypes;



namespace PasswordHash;
public class UserDTo //DTo class to used to return necessary data from API
{
    public int Id { get; private set; }
    public string Username { get; private set; }

    public UserDTo(int id, string username)
    {
        Id = id;
        Username = username;
    }


}
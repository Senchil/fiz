using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using fiz.Models;

namespace fiz.Data
{
    public static class SQLiteHelper
    {
        public static string ConnectionString { get; set; }

        static SQLiteHelper()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");
            ConnectionString = $"Data Source={dbPath};Version=3;";
            CreateDatabase();
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile("database.db");

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Таблица пользователей
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Login TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL
                    )";

                // Таблица студентов
                string createStudentsTable = @"
                    CREATE TABLE IF NOT EXISTS Students (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Faculty TEXT NOT NULL,
                        Group TEXT NOT NULL,
                        StudentCardNumber TEXT NOT NULL,
                        BirthDate TEXT NOT NULL,
                        ContactInfo TEXT NOT NULL
                    )";

                // Таблица мероприятий
                string createEventsTable = @"
                    CREATE TABLE IF NOT EXISTS Events (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Date TEXT NOT NULL,
                        Location TEXT NOT NULL,
                        Organizer TEXT NOT NULL,
                        SportType TEXT NOT NULL,
                        ParticipantCount INTEGER NOT NULL
                    )";

                // Таблица участий
                string createParticipationsTable = @"
                    CREATE TABLE IF NOT EXISTS Participations (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EventName TEXT NOT NULL,
                        StudentName TEXT NOT NULL,
                        Result TEXT NOT NULL,
                        Award TEXT NOT NULL,
                        Rank TEXT NOT NULL,
                        AddedBy TEXT NOT NULL,
                        Date TEXT NOT NULL
                    )";

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createUsersTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createStudentsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createEventsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createParticipationsTable;
                    command.ExecuteNonQuery();
                }

                // Добавляем админа по умолчанию
                string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Login = 'admin'";
                using (var command = new SQLiteCommand(checkAdmin, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count == 0)
                    {
                        string insertAdmin = "INSERT INTO Users (Login, Password) VALUES ('admin', 'admin')";
                        using (var insertCommand = new SQLiteCommand(insertAdmin, connection))
                        {
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }

                // Добавляем тестовые данные
                InsertTestData(connection);
            }
        }

        private static void InsertTestData(SQLiteConnection connection)
        {
            // Проверяем есть ли данные
            string checkStudents = "SELECT COUNT(*) FROM Students";
            using (var command = new SQLiteCommand(checkStudents, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    // Добавляем студентов
                    string insertStudent1 = @"
                        INSERT INTO Students (FullName, Faculty, Group, StudentCardNumber, BirthDate, ContactInfo)
                        VALUES ('Черкасский Данил Сергеевич', 'ФМИЗ', 'ИИ-23', '1337', '04.02.2005', 'danil@gmail.com')";

                    string insertStudent2 = @"
                        INSERT INTO Students (FullName, Faculty, Group, StudentCardNumber, BirthDate, ContactInfo)
                        VALUES ('Петров Петр Петрович', 'ФИ', 'ИО-21', '1338', '31.01.2003', '88005553535')";

                    using (var cmd = new SQLiteCommand(insertStudent1, connection))
                        cmd.ExecuteNonQuery();

                    using (var cmd = new SQLiteCommand(insertStudent2, connection))
                        cmd.ExecuteNonQuery();

                    // Добавляем мероприятие
                    string insertEvent = @"
                        INSERT INTO Events (Name, Date, Location, Organizer, SportType, ParticipantCount)
                        VALUES ('Кубок КГПИ', '27.02.2026', 'СК ''Олимп''', 'Факультетский', 'Плавание', 25)";

                    using (var cmd = new SQLiteCommand(insertEvent, connection))
                        cmd.ExecuteNonQuery();

                    // Добавляем участие
                    string insertParticipation = @"
                        INSERT INTO Participations (EventName, StudentName, Result, Award, Rank, AddedBy, Date)
                        VALUES ('Кубок КГПИ', 'Черкасский Данил Сергеевич', 'Призовое место', 'Медаль', '1-ый разряд', 'ivanov', '05.03.2026')";

                    using (var cmd = new SQLiteCommand(insertParticipation, connection))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        // ========== МЕТОДЫ ДЛЯ РАБОТЫ С БД ==========

        public static User AuthenticateUser(string login, string password)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Login = @login AND Password = @password";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Login = reader["Login"].ToString(),
                                Password = reader["Password"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static bool RegisterUser(string login, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Users (Login, Password) VALUES (@login, @password)";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false; // Пользователь уже существует
            }
        }

        public static List<Student> GetAllStudents()
        {
            var students = new List<Student>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Students";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FullName = reader["FullName"].ToString(),
                            Faculty = reader["Faculty"].ToString(),
                            Group = reader["Group"].ToString(),
                            StudentCardNumber = reader["StudentCardNumber"].ToString(),
                            BirthDate = reader["BirthDate"].ToString(),
                            ContactInfo = reader["ContactInfo"].ToString()
                        });
                    }
                }
            }
            return students;
        }

        public static List<Event> GetAllEvents()
        {
            var events = new List<Event>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Events";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new Event
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Date = reader["Date"].ToString(),
                            Location = reader["Location"].ToString(),
                            Organizer = reader["Organizer"].ToString(),
                            SportType = reader["SportType"].ToString(),
                            ParticipantCount = Convert.ToInt32(reader["ParticipantCount"])
                        });
                    }
                }
            }
            return events;
        }

        public static List<Participation> GetAllParticipations()
        {
            var participations = new List<Participation>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Participations";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        participations.Add(new Participation
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            EventName = reader["EventName"].ToString(),
                            StudentName = reader["StudentName"].ToString(),
                            Result = reader["Result"].ToString(),
                            Award = reader["Award"].ToString(),
                            Rank = reader["Rank"].ToString(),
                            AddedBy = reader["AddedBy"].ToString(),
                            Date = reader["Date"].ToString()
                        });
                    }
                }
            }
            return participations;
        }

        public static void AddStudent(Student student)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO Students (FullName, Faculty, Group, StudentCardNumber, BirthDate, ContactInfo)
                    VALUES (@FullName, @Faculty, @Group, @StudentCardNumber, @BirthDate, @ContactInfo)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", student.FullName);
                    command.Parameters.AddWithValue("@Faculty", student.Faculty);
                    command.Parameters.AddWithValue("@Group", student.Group);
                    command.Parameters.AddWithValue("@StudentCardNumber", student.StudentCardNumber);
                    command.Parameters.AddWithValue("@BirthDate", student.BirthDate);
                    command.Parameters.AddWithValue("@ContactInfo", student.ContactInfo);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddEvent(Event ev)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO Events (Name, Date, Location, Organizer, SportType, ParticipantCount)
                    VALUES (@Name, @Date, @Location, @Organizer, @SportType, @ParticipantCount)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", ev.Name);
                    command.Parameters.AddWithValue("@Date", ev.Date);
                    command.Parameters.AddWithValue("@Location", ev.Location);
                    command.Parameters.AddWithValue("@Organizer", ev.Organizer);
                    command.Parameters.AddWithValue("@SportType", ev.SportType);
                    command.Parameters.AddWithValue("@ParticipantCount", ev.ParticipantCount);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddParticipation(Participation participation)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO Participations (EventName, StudentName, Result, Award, Rank, AddedBy, Date)
                    VALUES (@EventName, @StudentName, @Result, @Award, @Rank, @AddedBy, @Date)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EventName", participation.EventName);
                    command.Parameters.AddWithValue("@StudentName", participation.StudentName);
                    command.Parameters.AddWithValue("@Result", participation.Result);
                    command.Parameters.AddWithValue("@Award", participation.Award);
                    command.Parameters.AddWithValue("@Rank", participation.Rank);
                    command.Parameters.AddWithValue("@AddedBy", participation.AddedBy);
                    command.Parameters.AddWithValue("@Date", participation.Date);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
namespace api.dbl.repo
{
    using api.models;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Dapper;
    using System.Data;
    using Npgsql;
    using Microsoft.Extensions.Logging;
    using api.dbl.repo.interfaces;

    public class UserRepo : IUserRepo
    {
        private string connectionString;

        public UserRepo(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("ConnectionStrings:developmentConnection");

        }
        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }

        public void Add(User item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute(@"INSERT INTO er.User (Username) 
                                    VALUES(@username)", item);
            }

        }

        public void AddNewUser(UserViewModel data)
        {  
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Query($@"CALL er.csp_create_new_user(
                    '{data.username}'::varchar, '{data.password}'::varchar, 
                    '{data.roleselected}'::varchar, {data.isauthor}::boolean, 
                    '{data.email}'::varchar)"
                );
            }

        }
        public string getConnectionString()
        {
            return $"ConnectionString: {connectionString}";
        }
        public IEnumerable<User> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {

                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM er.User");
            }
        }

        public User FindByID(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM er.User WHERE Userid = @Id", new { Id = id }).FirstOrDefault();
            }
        }

        public void Remove(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM er.User WHERE Userid=@Id", new { Id = id });
            }
        }

        public void Update(User item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Query("UPDATE er.User SET username = @username WHERE userid = @userid", item);
            }
        }

        public bool UserLogin(UserLogin login)
        {
              using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<bool>($"select * from er.csp_user_login('{login.username}'::varchar, '{login.password}'::varchar);").FirstOrDefault();
            }
        }
    }
}
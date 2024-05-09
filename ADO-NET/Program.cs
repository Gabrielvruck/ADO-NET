using ADO_NET.Models;
using Dapper;
using Microsoft.Data.SqlClient;

const string connectionString
    = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;TrustServerCertificate=true;";

//DAPPER
using (var connection = new SqlConnection(connectionString))
{

    //ListCategores(connection);
    //CreateCategory(connection);
    //CreateManyCategory(connection);
    //AdoNet();
    //UpdateCategory(connection);
    //ExecuteProcedure(connection);
    //ExecuteReadProcedure(connection);
    //ExecuteScalar(connection);
    //ReadView(connection);
    //OneToOne(connection);
    //OneToMany(connection);
    //QueryMutiple(connection);
    //SelectIn(connection);   
    //SelectLike(connection);
    Transactions(connection);

}

static void ListCategores(SqlConnection connection)
{
    var categories = connection.Query<Category>("SELECT [Id], [Title], [Url] FROM [Category]");
    foreach (var item in categories)
    {
        Console.WriteLine($"Id - {item.Id}");
        Console.WriteLine($"Titulo - {item.Title}");
        Console.WriteLine($"Url - {item.Url}");
    }
}

static void CreateCategory(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Amazon AWS";
    category.Url = "Amazon";
    category.Summary = "AWS Cloud";
    category.Order = 8;
    category.Description = "Categoria destinada a serviço do aws";
    category.Featured = false;
    //SQL INJECTION

    var insertSql = @"INSERT INTO [Category]
    VALUES(
    @Id,
    @Title,
    @Url,
    @Summary,
    @Order,
    @Description,
    @Featured)";

    var rows = connection.Execute(insertSql, new
    {
        category.Id,
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
    });
    Console.WriteLine($"Linhas inseridas - {rows}");
}

static void CreateManyCategory(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Amazon AWS 2";
    category.Url = "Amazon 2";
    category.Summary = "AWS Cloud 2";
    category.Order = 10;
    category.Description = "Categoria destinada a serviço do aws 2";
    category.Featured = false;

    var category2 = new Category();
    category2.Id = Guid.NewGuid();
    category2.Title = "Categoria nova";
    category2.Url = "Categoria-nova";
    category2.Summary = "Nova Categoria";
    category2.Order = 9;
    category2.Description = "Categoria";
    category2.Featured = false;
    //SQL INJECTION

    var insertSql = @"INSERT INTO [Category]
    VALUES(
    @Id,
    @Title,
    @Url,
    @Summary,
    @Order,
    @Description,
    @Featured)";

    var rows = connection.Execute(insertSql, new[]
    {
       new {
        category.Id,
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
       },
       new {
        category2.Id,
        category2.Title,
        category2.Url,
        category2.Summary,
        category2.Order,
        category2.Description,
        category2.Featured
       },
    });
    Console.WriteLine($"Linhas inseridas - {rows}");
}

static void UpdateCategory(SqlConnection connection)
{
    var updateQuery = "UPDATE [Category] SET [Title] = @Title WHERE [Id] = @Id";
    var rows = connection.Execute(updateQuery, new
    {
        Id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
        Title = "Frontend 2024"
    });

    Console.WriteLine($"Registro Atualizadas - {rows}");
}

static void AdoNet()
{
    //ado.net
    using (var connectionAdoNet = new SqlConnection(connectionString))
    {
        Console.WriteLine("Conectado");
        connectionAdoNet.Open();

        using (var command = new SqlCommand())
        {
            command.Connection = connectionAdoNet;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "SELECT [Id], [Title] FROM [Category]";

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //coluna das propriedade Id, Title;
                Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
            }

        }
    }
}

static void ExecuteProcedure(SqlConnection connection)
{
    var procedure = "[spDeleteStudent]";
    var pars = new { StudentId = "79b82071-80a8-4e78-a79c-92c8cd1fd052" };
    var affectedRows = connection.Execute(procedure, pars, commandType: System.Data.CommandType.StoredProcedure);
    Console.WriteLine($"Linhas afetadas - {affectedRows}");
}

static void ExecuteReadProcedure(SqlConnection connection)
{
    var procedure = "[spListCourse]";
    var pars = new { Category = "Frontend 2024" };
    var Courses = connection.Query(procedure, pars, commandType: System.Data.CommandType.StoredProcedure);

    foreach (var c in Courses)
    {
        Console.WriteLine($"ID - {c.Id} / Titulo - {c.Title}");
    }

}

static void ExecuteScalar(SqlConnection connection)
{
    var category = new Category();
    category.Title = "Amazon AWS";
    category.Url = "Amazon";
    category.Summary = "AWS Cloud";
    category.Order = 8;
    category.Description = "Categoria destinada a serviço do aws";
    category.Featured = false;
    //SQL INJECTION

    var insertSql = @"INSERT INTO [Category]
    OUTPUT Inserted.[Id]
    VALUES(
    NEWID(),
    @Title,
    @Url,
    @Summary,
    @Order,
    @Description,
    @Featured)";

    var id = connection.ExecuteScalar<Guid>(insertSql, new
    {
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
    });
    Console.WriteLine($"Categoria inserida foi: - {id}");
}

static void ReadView(SqlConnection connection)
{
    var sql = "SELECT * FROM [vwCourses]";
    var courses = connection.Query(sql);
    foreach (var c in courses)
    {
        Console.WriteLine($" - {c.Id}- titulo - {c.Title}");
    }
}

static void OneToOne(SqlConnection connection)
{
    var sql = @"SELECT * FROM [CareerItem]
    INNER JOIN [Course] ON [CareerItem].[CourseId] = [Course].[Id]";
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    var items = connection.Query<CareerItem, Course, CareerItem>(sql, (careerItem, course) =>
    {
        careerItem.Course = course;
        return careerItem;
    }, splitOn: "Id");

    foreach (var item in items)
    {
        Console.WriteLine($"{item.Id} - Titulo career-{item.Title} Titulo curso - {item.Course.Title}");
    }
}

static void OneToMany(SqlConnection connection)
{
    var sql = @"SELECT
    Career.Id,
    Career.Title,
    CareerItem.CareerId,
    CareerItem.Title
    FROM 
    Career
    INNER JOIN
    CareerItem ON CareerItem.CareerId = Career.Id
    ORDER BY Career.Title";

    var careers = new List<Career>();
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    var items = connection.Query<Career, CareerItem, Career>(sql, (career, item) =>
    {
        var car = careers.Where(c => c.Id == career.Id).FirstOrDefault();
        if (car == null)
        {
            car = career;
            car.Items.Add(item);
            careers.Add(car);
        }
        else
        {
            car.Items.Add(item);
        }

        return career;
    }, splitOn: "CareerId");

    foreach (var career in items)
    {
        Console.WriteLine($"{career.Title}");
        foreach (var item in career.Items)
        {
            Console.WriteLine($" - {item.Title} ");
        }
    }
}

static void QueryMutiple(SqlConnection connection)
{
    var query = "SELECT * FROM CATEGORY; SELECT * FROM COURSE";
    using (var mult = connection.QueryMultiple(query))
    {
        var category = mult.Read<Category>();
        var course = mult.Read<Course>();

        foreach (var item in category)
        {
            Console.WriteLine(item.Title);
        }

        foreach (var item in course)
        {
            Console.WriteLine(item.Title);
        }
    }
}


static void SelectIn(SqlConnection connection)
{
    var query = @"SELECT * FROM [Category] WHERE [Id] IN @Id";
    var categories = connection.Query<Category>(query, new
    {
        id = new[]
        {
            "af3407aa-11ae-4621-a2ef-2028b85507c4",
            "6b99eb0a-da5e-41ad-b814-22d4c1fe1dfb"
        }
    });
    foreach (var item in categories)
    {
        Console.WriteLine($"Id - {item.Id}");
        Console.WriteLine($"Titulo - {item.Title}");
        Console.WriteLine($"Url - {item.Url}");
    }
}

static void SelectLike(SqlConnection connection)
{
    var term = "amazon";
    var query = @"SELECT * FROM [Category] WHERE [Title] like @exp ";
    var categories = connection.Query<Category>(query, new
    {
        exp = $"%{term}%"
    });
    foreach (var item in categories)
    {
        Console.WriteLine($"Id - {item.Id}");
        Console.WriteLine($"Titulo - {item.Title}");
        Console.WriteLine($"Url - {item.Url}");
    }
}

static void Transactions(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Minha categoria vruck";
    category.Url = "vruck";
    category.Summary = "vruck Cloud";
    category.Order = 8;
    category.Description = "Categoria vruck programeitor";
    category.Featured = false;
    //SQL INJECTION

    var insertSql = @"INSERT INTO [Category]
    VALUES(
    @Id,
    @Title,
    @Url,
    @Summary,
    @Order,
    @Description,
    @Featured)";
    connection.Open();
    using (var transaction = connection.BeginTransaction())
    {
        var rows = connection.Execute(insertSql, new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        }, transaction);

        transaction.Commit();
        //não salvar alteração
        //transaction.Rollback();
        Console.WriteLine($"Linhas inseridas - {rows}");
    }
}
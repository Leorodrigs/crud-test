using DocumentationDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentationDemo.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    private static readonly object SyncRoot = new();

    private static readonly List<User> Users =
    [
        new() { Id = 1, Nome = "João", Sobrenome = "Silva" },
        new() { Id = 2, Nome = "Maria", Sobrenome = "Souza" }
    ];

    private static int _nextId = 3;

    [HttpPost(Name = "CreateUser")]
    [EndpointSummary("Create a user")]
    [EndpointDescription("Creates a user in the in-memory collection and generates its identifier.")]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<User> Create(UserInput input)
    {
        User user;

        lock (SyncRoot)
        {
            user = new User
            {
                Id = _nextId++,
                Nome = input.Nome,
                Sobrenome = input.Sobrenome
            };

            Users.Add(user);
        }

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}", Name = "UpdateUser")]
    [EndpointSummary("Update a user")]
    [EndpointDescription("Updates the name of an existing user in the in-memory collection.")]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> Update(int id, UserInput input)
    {
        lock (SyncRoot)
        {
            var user = Users.FirstOrDefault(item => item.Id == id);

            if (user is null)
            {
                return NotFound();
            }

            user.Nome = input.Nome;
            user.Sobrenome = input.Sobrenome;

            return Ok(user);
        }
    }

    [HttpDelete("{id:int}", Name = "DeleteUser")]
    [EndpointSummary("Delete a user")]
    [EndpointDescription("Removes a user from the in-memory collection.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        lock (SyncRoot)
        {
            var user = Users.FirstOrDefault(item => item.Id == id);

            if (user is null)
            {
                return NotFound();
            }

            Users.Remove(user);
            return NoContent();
        }
    }

    [HttpGet(Name = "GetUsers")]
    [EndpointSummary("List users")]
    [EndpointDescription("Returns every user currently stored in memory.")]
    [ProducesResponseType<IReadOnlyList<User>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<User>> GetAll()
    {
        lock (SyncRoot)
        {
            return Ok(Users.ToArray());
        }
    }

    [HttpGet("{id:int}", Name = "GetUserById")]
    [EndpointSummary("Get a user by identifier")]
    [EndpointDescription("Returns the user with the specified identifier.")]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetById(int id)
    {
        lock (SyncRoot)
        {
            var user = Users.FirstOrDefault(item => item.Id == id);
            return user is null ? NotFound() : Ok(user);
        }
    }
}

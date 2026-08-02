using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Creates a new game.
    /// If the mode is not specified, it defaults to TwoPlayer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Game), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<Game> CreateGame([FromQuery] GameMode mode = GameMode.TwoPlayer)
    {
        var game = _gameService.CreateGame(mode);

        return CreatedAtAction(
            nameof(GetGame),
            new { id = game.Id },
            game);
    }

    /// <summary>
    /// Returns current game state associated with the provided game ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Game> GetGame(Guid id)
    {
        var game = _gameService.GetGame(id);
        return Ok(game);
    }

    /// <summary>
    /// Requests a move in the game associated with the provided game ID.
    /// Moverequest should include the player making the move and the position (row and column) on the board.
    /// </summary>
    [HttpPost("{id:guid}/moves")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Game> MakeMove(
        Guid id,
        [FromBody] MoveRequest request)
    {
        var game = _gameService.MakeMove(id, request);
        return Ok(game);
    }


        /// <summary>
        /// Undo last move.
        /// </summary>
        [HttpPost("{id:guid}/undo")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Game> Undo(Guid id)
    {
        return Ok(_gameService.Undo(id));
    }

    /// <summary>
    /// Reset current game.
    /// </summary>
    [HttpPost("{id:guid}/reset")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Game> Reset(Guid id)
    {
        return Ok(_gameService.ResetGame(id));
    }
}
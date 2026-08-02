using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly IGameService _gameService;

    public ScoreboardController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Gets current scoreboard.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Scoreboard), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<Scoreboard> GetScoreboard()
    {
        return Ok(_gameService.GetScoreboard());
    }

    /// <summary>
    /// Resets scoreboard.
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(Scoreboard), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<Scoreboard> ResetScoreboard()
    {
        _gameService.ResetScoreboard();

        return Ok(_gameService.GetScoreboard());
    }
}
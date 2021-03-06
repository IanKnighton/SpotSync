﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotSync.Application.Services;
using SpotSync.Domain;
using SpotSync.Domain.Contracts;
using SpotSync.Domain.Contracts.Services;
using SpotSync.Domain.DTO;
using SpotSync.Models.Dashboard;
using SpotSync.Models.Shared;

namespace SpotSync.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IPartyService _partyService;
        private readonly IPartyGoerService _partyGoerService;
        private readonly Random _random;
        private readonly ILogService _logService;

        static private readonly string[] GREETINGS = new string[]
            {
                "Ello mate",
                "Howdy",
                "What's up",
                "Hey",
                "Aloha",
                "Sup",
                "Yo",
                "Greetings",
                "Ahoy",
                "Salutations",
                "Namaste",
                "Bonjour",
                "Que pasa",
                "Konnichiwa",
                "Ciao"
            };

        public DashboardController(IPartyService partyService, IPartyGoerService partyGoerService, ILogService logService)
        {
            _partyService = partyService;
            _partyGoerService = partyGoerService;
            _logService = logService;
            _random = new Random();
        }

        [Authorize]
        public async Task<IActionResult> Index(string errorMessage)
        {
            try
            {
                PartyGoer user = new PartyGoer(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Party party = await _partyService.GetPartyWithAttendeeAsync(user);

                List<Domain.Song> userRecommendedSongs = await _partyGoerService.GetRecommendedSongsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

                List<Party> topParties = await _partyService.GetTopParties(3);

                DashboardModel model = new DashboardModel
                {
                    Name = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    AvailableParties = topParties.Select(p => new PreviewPartyModel
                    {
                        AlbumArtUrl = p.Playlist?.CurrentSong?.AlbumImageUrl,
                        ListenerCount = p.Listeners.Count,
                        Name = "Default Party Name",
                        PartyCode = p.PartyCode
                    }).ToList(),
                    SuggestedSongs = userRecommendedSongs.Select(p => new PreviewPlaylistSong { Artist = p.Artist, Title = p.Title, TrackUri = p.TrackUri, Selected = true }).ToList(),
                    RandomGreeting = GetRandomGreeting()
                };

                BaseModel baseModel = new BaseModel(party == null ? false : true, party?.PartyCode, errorMessage);

                return View(new BaseModel<DashboardModel>(model, baseModel));
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, "Error occurred in Index()");
                // Todo: Add error view
                return View();
            }
        }

        private string GetRandomGreeting()
        {
            return GREETINGS[_random.Next(0, GREETINGS.Length - 1)];
        }
    }
}

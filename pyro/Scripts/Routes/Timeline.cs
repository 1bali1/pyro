using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Timeline : ControllerBase
    {
        private readonly MItemshop _itemshop;
        public Timeline(MItemshop itemshop)
        {
            _itemshop = itemshop;
        }

        [HttpGet("fortnite/api/calendar/v1/timeline")]
        public async Task<IActionResult> CalendarTimeline()
        {
            var currentEvents = new[]
            {
                new
                {
                    eventType = $"EventFlag.Season{Utils.Utils.seasonNumber}",
                    activeUntil = "9999-01-01T00:00:00.000Z",
                    activeSince = "2020-12-03T00:00:00.000Z"
                },
                new
                {
                    eventType = $"EventFlag.LobbySeason{Utils.Utils.seasonNumber}",
                    activeUntil = "9999-01-01T00:00:00.000Z",
                    activeSince = "2020-12-03T00:00:00.000Z"
                }
            };

            var jsonData = new
            {
                channels = new
                {
                    client_matchmaking = new
                    {
                        states = Array.Empty<object>(),
                        cacheExpire = "9999-01-01T00:00:00.000Z"
                    },
                    client_events = new
                    {
                        states = new[] {
                            new {
                                activeEvents = currentEvents,
                                validFrom = "2020-12-03T00:00:00.000Z",
                                state = new {
                                    activeStorefronts = Array.Empty<object>(),
                                    eventNamedWeights = new { },
                                    seasonNumber = Utils.Utils.seasonNumber,
                                    seasonTemplateId = $"AthenaSeason:athenaseason{Utils.Utils.seasonNumber}",
                                    matchXpBonusPoints = 0,
                                    seasonBegin = "2020-12-03T00:00:00Z",
                                    seasonEnd = "9999-01-01T00:00:00Z",
                                    seasonDisplayedEnd = "9999-01-01T00:00:00Z",
                                    weeklyStoreEnd = "9999-01-02T00:00:00.000Z",
                                    stwEventStoreEnd = "9999-01-01T00:00:00.000Z",
                                    stwWeeklyStoreEnd = "9999-01-01T00:00:00.000Z",
                                    sectionStoreEnds = new {
                                        Featured = "9999-01-01T00:00:00.000Z"
                                    },
                                    dailyStoreEnd = !_itemshop.autoItemshop.enabled
                                        ? "9999-01-02T00:00:00.000Z"
                                        : DateTime.UtcNow.ToString("yyyy-MM-ddT23:59:59Z")
                                }
                            }
                        },
                        cacheExpire = "9999-01-01T00:00:00.000Z"
                    }
                },
                eventsTimeOffsetHrs = 0,
                cacheIntervalMins = 10,
                currentTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            return Ok(jsonData);
        }
    }
}
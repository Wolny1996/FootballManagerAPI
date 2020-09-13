using FootballManager.API.DTOs.Transfer;
using System;
using System.Collections.Generic;

namespace FootballManager.API.DTOs.Response
{
    public class ClubResponse
    {
        public string ClubName { get; set; }
        public string City { get; set; }
        public DateTime Founded { get; set; }
        public int Age { get; set; }
        public StadiumDto Stadium { get; set; }
        public CoachDto Coach { get; set; }
        public IEnumerable<string> Footballers { get; set; }
        public IEnumerable<string> Tournaments { get; set; }
    }
}
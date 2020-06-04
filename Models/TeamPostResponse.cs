using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace TeamAppService.Models
{
    public class TeamPostResponse
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public DateTime date { get; set; }

        public TeamPostResponse(Team team)
        {
            this.id = team.id;
            this.title = team.title;
            this.name = team.name;
            this.date = team.date;
        }
    }
}

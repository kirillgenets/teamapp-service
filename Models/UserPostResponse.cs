using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace TeamAppService.Models
{
    public class UserPostResponse
    {
        public long id { get; set; }
        [DataMember]
        public string login { get; set; }
        [DataMember]
        public DateTime date { get; set; }
        [DataMember]
        public long teamId { get; set; }

        public UserPostResponse(PostUser user)
        {
            this.id = user.id;
            this.login = user.login;
            this.date = user.date;
            this.teamId = user.teamId;
        }

        public UserPostResponse(User user)
        {
            this.id = user.id;
            this.login = user.login;
            this.date = user.date;
            this.teamId = user.teamId;
        }
    }
}

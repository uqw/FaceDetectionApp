using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.Model.Recognition
{
    public class User
    {
        public int Id { get; }
        public string Username { get; }

        public User(int id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}

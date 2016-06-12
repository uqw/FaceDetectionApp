using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using FaceDetection.ViewModel;

namespace FaceDetection.Model.Recognition
{
    internal class RecognitionData
    {
        public RecognitionData()
        {
            
        }

        public async Task<List<Face>> GetAllFaces()
        {
            var list = new List<Face>();

            using (var result = await DatabaseHandler.SelectAsync("SELECT f.id as id, f.grayframe as grayframe, f.original as original, f.userID as userID, u.username as username, f.width as width, f.height as height FROM faces f, users u WHERE u.id = f.userID"))
            {
                if (result == null)
                    return list;

                while (result.Read())
                {
                    list.Add(new Face
                    (
                        (byte[])result["original"],
                        (byte[]) result["grayframe"],
                        Convert.ToInt32(result["id"]),
                        (string)result["username"],
                        Convert.ToInt32(result["userID"]),
                        Convert.ToInt32(result["width"]),
                        Convert.ToInt32(result["height"])
                    ));
                }
            }

            return list;
        }

        public async Task<long> InsertFace(byte[] original, byte[] grayframe, string username, int width, int height)
        {
            var userId = await DatabaseHandler.InsertAsync("INSERT INTO users (username) VALUES (@username)", new SQLiteParameter("@username", username));

            var faceId = await DatabaseHandler.InsertAsync("INSERT INTO faces (original, grayframe, userID, width, height) VALUES (@original, @grayframe, @userId, @width, @height)",
                new SQLiteParameter("@original", DbType.Binary)
                {
                    Value = original
                },
                new SQLiteParameter("@grayframe", DbType.Binary)
                {
                    Value = grayframe
                },
                new SQLiteParameter("@userId", userId),
                new SQLiteParameter("@width", width),
                new SQLiteParameter("@height", height)
            );

            return faceId;
        }
    }
}

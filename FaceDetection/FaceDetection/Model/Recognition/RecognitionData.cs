using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace FaceDetection.Model.Recognition
{
    internal static class RecognitionData
    {
        public static ObservableCollection<Face> AllFaces { get; private set; }
        public static List<User> AllUsers { get; private set; }

        static RecognitionData()
        {
            InitFaces();
            InitUsers();
        }

        private static void AllFacesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            RecognitionEngine.TrainRecognizer(AllFaces.ToList());
        }

        public static async void InitUsers()
        {
            AllUsers = await GetAllUsers();
        }

        public static async void InitFaces()
        {
            AllFaces = new ObservableCollection<Face>(await GetAllFaces());
            AllFaces.CollectionChanged += AllFacesOnCollectionChanged;
        }

        public static User GetUser(int id)
        {
            return AllUsers.Find(user => user.Id == id);
        }

        public static async Task<List<User>> GetAllUsers()
        {
            var list = new List<User>();

            using (var result = await DatabaseHandler.SelectAsync("SELECT id, username, firstname, lastname FROM users"))
            {
                if (result == null)
                    return list;

                while(result.Read())
                {
                    list.Add(new User(
                        Convert.ToInt32(result["id"]),
                        (string) result["username"],
                        (string) result["firstname"],
                        (string) result["lastname"]
                        ));
                }
            }

            return list;
        }

        public static async Task<List<Face>> GetAllFaces()
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

        public static async Task<AddedFaceData> InsertFace(Image<Bgr, byte> original, Image<Gray, byte> grayframe, string username)
        {
            var size = Properties.Settings.Default.RecognitionImageSize;

            original = original.Resize(size, size, Inter.Cubic);
            grayframe = grayframe.Resize(size, size, Inter.Cubic);

            var userId = await DatabaseHandler.InsertAsync("INSERT INTO users (username) VALUES (@username)", new SQLiteParameter("@username", username));

            var faceId = await DatabaseHandler.InsertAsync("INSERT INTO faces (original, grayframe, userID, width, height) VALUES (@original, @grayframe, @userId, @width, @height)",
                new SQLiteParameter("@original", DbType.Binary)
                {
                    Value = original.Bytes
                },
                new SQLiteParameter("@grayframe", DbType.Binary)
                {
                    Value = grayframe.Bytes
                },
                new SQLiteParameter("@userId", userId),
                new SQLiteParameter("@width", original.Width),
                new SQLiteParameter("@height", original.Height)
            );

            AllFaces.Add(new Face(original, grayframe, (int)faceId, username, (int)userId));

            AllUsers.Add(new User((int)userId, username));

            return new AddedFaceData(userId, faceId);
        }
    }
}

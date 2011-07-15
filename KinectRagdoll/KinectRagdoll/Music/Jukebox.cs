using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace KinectRagdoll.Music
{
    /// <summary>
    /// Music by Kevin MacLeod at http://www.incompetech.com
    /// </summary>
    public class Jukebox
    {
        private static Dictionary<String, Song> playlist = new Dictionary<string, Song>();


        public static List<String> Playlist
        {
            get
            {
                return playlist.Keys.ToList();
            }
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            playlist.Add("Amazing Plan", Content.Load<Song>("Music\\AmazingPlan"));
            playlist.Add("Fig Leaf Rag", Content.Load<Song>("Music\\FigLeafRag"));
            playlist.Add("Dark Hallway", Content.Load<Song>("Music\\DarkHallway"));


        }

        public static void Play(String song)
        {
            Song newSong;
            if (playlist.TryGetValue(song, out newSong))
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(newSong);
            }

        }
    }
}

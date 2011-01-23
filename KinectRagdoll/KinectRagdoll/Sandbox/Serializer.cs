using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace KinectRagdoll.Sandbox
{
    class Serializer
    {

        public static SaveFile readFromDataContract(String filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlDictionaryReaderQuotas q = new XmlDictionaryReaderQuotas();
            q.MaxDepth = 1000;

            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs, q);
            DataContractSerializer ser = new DataContractSerializer(
                typeof(SaveFile), null, Int32.MaxValue, false, true, null);

            // Deserialize the data and read it from the instance.
            SaveFile g =
                (SaveFile)ser.ReadObject(reader, true);
            reader.Close();
            fs.Close();

            return g;
        }

        public static void Save(World w, KinectRagdollGame g)
        {

            FileStream writer = new FileStream("save.xml", FileMode.Create);

            SaveFile sf = new SaveFile();
            foreach (Body b in w.BodyList) {
                if (!g.ragdollManager.OwnsBody(b))
                {
                    sf.bodyList.Add(b);
                }
            }

            foreach (Joint j in w.JointList)
            {
                if (!g.ragdollManager.OwnsJoint(j))
                {
                    sf.jointList.Add(j);
                }
            }
            
            
            sf.gravity = w.Gravity;
 
            DataContractSerializer ser = new DataContractSerializer(
                typeof(SaveFile), null, Int32.MaxValue, false, true, null);
            ser.WriteObject(writer, sf);
            writer.Close();

        }




    }

    [DataContract(Name = "SaveFile", Namespace = "http://www.imcool.com")]
    public class SaveFile
    {
        [DataMember()]
        public List<Joint> jointList = new List<Joint>();
        [DataMember()]
        public List<Body> bodyList = new List<Body>();
        [DataMember()]
        public Vector2 gravity;


        public World createWorld()
        {
            World w = new World(gravity);

            return w;

        }


        public void PopulateWorld(World w) {


            foreach (Body b in bodyList) {
                
                b.setWorld(w);
                

                foreach (Fixture f in b.FixtureList)
                {
                    f.Shape.ComputeProperties();
                    w.FixtureAdded(f);
                }

                b.ResetMassData();
                b.Enabled = true;
                b.Awake = true;
                w.AddBody(b);
                //w.BodyList.Add(b);
            }

            w.ProcessChanges();

            foreach (Joint j in jointList)
            {
                w.AddJoint(j);
            }

            w.ProcessChanges();

        }

    }


}

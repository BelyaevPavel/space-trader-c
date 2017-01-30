using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Summary
{
    /// <summary>
    /// Summary description for Vector3.
    /// </summary>
    public class Vector3
    {
        double x;
        double y;
        double z;
        double length;

        [JsonProperty ("x")]
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                ComputeLength ();
            }
        }

        [JsonProperty ("y")]
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                ComputeLength ();
            }
        }

        [JsonProperty ("z")]
        public double Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
                ComputeLength ();
            }
        }

        public double Length
        {
            get
            {
                return length;
            }
        }

        // Constructors
        [JsonConstructor]
        public Vector3(double X, double Y, double Z)
        {
            x = X;
            y = Y;
            z = Z;
            ComputeLength ();
        }

        public Vector3() : this (0, 0, 0) { }

        public Vector3(Vector3 v) : this (v.X, v.Y, v.Z) { }
        //
        /// <summary>
        /// Computes the length of the vector and stores it.
        /// This function is automatically called whenever one of
        /// the vector components is changed.
        /// </summary>
        void ComputeLength()
        {
            length = Math.Sqrt (Math.Pow (x, 2) + Math.Pow (y, 2) + Math.Pow (z, 2));
        }

        public static double CalculateLength(Vector3 v)
        {
            return Math.Sqrt (Math.Pow (v.x, 2) + Math.Pow (v.x, 2) + Math.Pow (v.x, 2));
        }
        // Overloaded Operators
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3 (v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3 (v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator +(Vector3 v)
        {
            return new Vector3 (v);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3 (-v.X, -v.Y, -v.Z);
        }

        public static Vector3 operator *(Vector3 v1, double x)
        {
            return new Vector3 (v1.X * x, v1.Y * x, v1.Z * x);
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            return (this == (Vector3) obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode ();
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            if (!(v1 == v2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Computes the dot (scalar)-product of two 3-dimensional vectors
        /// </summary>
        public static double operator %(Vector3 v1, Vector3 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        /// <summary>
        /// Computes the cross (vector)-product of two 3-dimensional vectors
        /// </summary>
        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3 (v1.Y * v2.Z - v1.Z * v2.Y, v1.X * v2.Z - v1.Z * v2.X, v1.X * v2.Y -
                v1.Y * v2.X);
        }

        /// <summary>
        /// Returns the unit vector of a given vector.
        /// </summary>
        public static Vector3 operator ~(Vector3 v)
        {
            return new Vector3 (v.X / v.Length, v.Y / v.Length, v.Z / v.Length);
        }
        //
        // Basis vectors for 3d-coordinate system
        public static Vector3 BasisI
        {
            get
            {
                return new Vector3 (1, 0, 0);
            }
        }
        public static Vector3 BasisJ
        {
            get
            {
                return new Vector3 (0, 1, 0);
            }
        }
        public static Vector3 BasisK
        {
            get
            {
                return new Vector3 (0, 0, 1);
            }
        }
        //
        public double this [long index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    case 3:
                        return length;
                    default:
                        throw new System.IndexOutOfRangeException
                            ("Index must fall between 0 and 3");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException
                            ("Index must fall between 0 and 2");
                }
                ComputeLength ();
            }
        }

        public override string ToString()
        {
            return x.ToString () + " " + y.ToString () + " " + z.ToString ();
        }

        public double GetAngle(Vector3 v1, Vector3 v2)
        {
            return (v1 % v2) / (v1.length * v2.length);
        }
    }

    public static class XmlHelper
    {
        public static bool NewLineOnAttributes
        {
            get; set;
        }
        /// <summary>
        /// Serializes an object to an XML string, using the specified namespaces.
        /// </summary>
        public static string ToXml(object obj, XmlSerializerNamespaces ns)
        {
            Type T = obj.GetType ();

            var xs = new XmlSerializer (T);
            var ws = new XmlWriterSettings { Indent = true, NewLineOnAttributes = NewLineOnAttributes, OmitXmlDeclaration = true };

            var sb = new StringBuilder ();
            using (XmlWriter writer = XmlWriter.Create (sb, ws))
            {
                xs.Serialize (writer, obj, ns);
            }
            return sb.ToString ();
        }

        /// <summary>
        /// Serializes an object to an XML string.
        /// </summary>
        public static string ToXml(object obj)
        {
            var ns = new XmlSerializerNamespaces ();
            ns.Add ("", "");
            return ToXml (obj, ns);
        }

        /// <summary>
        /// Deserializes an object from an XML string.
        /// </summary>
        public static T FromXml<T>(string xml)
        {
            XmlSerializer xs = new XmlSerializer (typeof (T));
            using (StringReader sr = new StringReader (xml))
            {
                return (T) xs.Deserialize (sr);
            }
        }

        /// <summary>
        /// Deserializes an object from an XML string, using the specified type name.
        /// </summary>
        public static object FromXml(string xml, string typeName)
        {
            Type T = Type.GetType (typeName);
            XmlSerializer xs = new XmlSerializer (T);
            using (StringReader sr = new StringReader (xml))
            {
                return xs.Deserialize (sr);
            }
        }

        /// <summary>
        /// Serializes an object to an XML file.
        /// </summary>
        public static void ToXmlFile(Object obj, string filePath)
        {
            var xs = new XmlSerializer (obj.GetType ());
            var ns = new XmlSerializerNamespaces ();
            var ws = new XmlWriterSettings { Indent = true, NewLineOnAttributes = NewLineOnAttributes, OmitXmlDeclaration = true };
            ns.Add ("", "");

            using (XmlWriter writer = XmlWriter.Create (filePath, ws))
            {
                xs.Serialize (writer, obj);
            }
        }

        /// <summary>
        /// Deserializes an object from an XML file.
        /// </summary>
        public static T FromXmlFile<T>(string filePath)
        {
            StreamReader sr = new StreamReader (filePath);
            try
            {
                var result = FromXml<T> (sr.ReadToEnd ());
                return result;
            }
            catch (Exception e)
            {
                throw new Exception ("There was an error attempting to read the file " + filePath + "\n\n" + e.InnerException.Message);
            }
            finally
            {
                sr.Close ();
            }
        }
    }

    public static class JsonHelper
    {
        public static void ToJsonFile(Object obj, string filePath)
        {
            using (StreamWriter file = new StreamWriter (filePath))
            {
                JsonSerializer serializer = new JsonSerializer ();
                serializer.Serialize (file, obj);
            }
        }

        public static T FromJsonFile<T>(string filePath)
        {
            StreamReader sr = new StreamReader (filePath);
            try
            {
                var result = FromJson<T> (sr.ReadToEnd ());
                return result;
            }
            catch (Exception e)
            {
                throw new Exception ("There was an error attempting to read the file " + filePath + "\n\n" + e.InnerException.Message);
                throw;
            }
            finally
            {
                sr.Close ();
            }
        }

        private static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T> (json);
        }
    }

}

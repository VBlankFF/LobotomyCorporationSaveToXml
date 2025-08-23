extern alias acs;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.Rendering;
using LobotomyBaseMod;

namespace XmlSaveMod
{
    public class Harmony_Patch
    {
        public static SerializableDictionary<string, object> dicttoser;
        public static XmlDocument save;
        public static XmlElement superElem;
        public static int docnum;
        public static Dictionary<string, Type> typeBuffer;
        private static Assembly gameAssembly;
        private static string filePath;
        private static bool fileExists;
        // Constructor
        public Harmony_Patch()
        {

            // Apply patches
            HarmonyInstance harmony = HarmonyInstance.Create("LobotomyCorporationSaveToXmlMod_VBlankFF");
            try
            {
                MethodInfo method = typeof(Harmony_Patch).GetMethod("LoadXMLPatch");
                harmony.Patch(typeof(SaveUtil).GetMethod("ReadSerializableFile", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("GetSaveType")), new HarmonyMethod(method), null);
                MethodInfo method2 = typeof(Harmony_Patch).GetMethod("SaveXMLPatch");
                harmony.Patch(typeof(SaveUtil).GetMethod("WriteSerializableFile", AccessTools.all), new HarmonyMethod(method2), null, null);

                //filePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\LobotomyCorp\\LobotomyCorp_Data\\BaseMods\\LobotomyArchipelago_MOD_v1.0.0\\saveData170808.xml";
                //SaveUtil.ReadSerializableFile("C:\\Users\\sterl\\AppData\\LocalLow\\Project_Moon\\Lobotomy\\saveData170808.dat");
                //filePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\LobotomyCorp\\LobotomyCorp_Data\\BaseMods\\LobotomyArchipelago_MOD_v1.0.0\\saveGlobal170808.xml";
                //SaveUtil.ReadSerializableFile("C:\\Users\\sterl\\AppData\\LocalLow\\Project_Moon\\Lobotomy\\saveGlobal170808.dat");
                //filePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\LobotomyCorp\\LobotomyCorp_Data\\BaseMods\\LobotomyArchipelago_MOD_v1.0.0\\saveUnlimitV5170808.xml";
                //SaveUtil.ReadSerializableFile("C:\\Users\\sterl\\AppData\\LocalLow\\Project_Moon\\Lobotomy\\saveUnlimitV5170808.dat");
                //filePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\LobotomyCorp\\LobotomyCorp_Data\\BaseMods\\LobotomyArchipelago_MOD_v1.0.0\\etc170808.xml";
                //SaveUtil.ReadSerializableFile("C:\\Users\\sterl\\AppData\\LocalLow\\Project_Moon\\Lobotomy\\etc170808.dat");

                fileExists = false;
                docnum = 0;
            }
            catch (Exception ex)
            {
                ////LobotomyBaseMod.ModDebug.Log(ex.ToString());
            }

        } // end constructor

        public static void GetSaveType(string fileName, ref string __state)
        {
            __state = fileName;
        }
        // Input
        public static void SaveExpoPatch(ref Dictionary<string, object> __result)
        {
            if (__result != null)
            {
                var y = __result;
                RecursiveOut(y, 0);
            }
        }
        public static void SaveXMLPatch(string fileName, Dictionary<string, object> dic)
        {
            save = new XmlDocument();
            filePath = "";
            superElem = save.CreateElement(null, "save", null);
            RecursiveXml(dic, superElem);
            save.AppendChild(superElem);
            //if (filePath.Equals(""))
            //{
            //    save.Save("C:\\Program Files (x86)\\Steam\\steamapps\\common\\LobotomyCorp\\LobotomyCorp_Data\\BaseMods\\LobotomyArchipelago_MOD_v1.0.0\\" + docnum.ToString() + ".xml");
            //}
            //else
            //{
            save.Save(fileName.Remove(fileName.Length - 4, 4) + ".xml");
            //}

            save = new XmlDocument();
            docnum++;
        }
        public static void LoadXMLPatch(ref Dictionary<string, object> __result, string __state)
        {
            try
            {
                if (__result != null)
                {

                    string adjustedFileName = __state.Remove(__state.Length - 4, 4) + ".xml";
                    if (gameAssembly is null)
                    {
                        gameAssembly = Assembly.GetAssembly(typeof(AgentModel));
                    }
                    if (File.Exists(adjustedFileName))
                    {
                        Dictionary<string, object> result = DeserializeXml(adjustedFileName);
                        __result = result;
                    }
                    else if (!fileExists)
                    {
                        string[] saveNames =
                        {
                            "saveData170808",
                            "saveGlobal170808",
                            "saveUnlimitV5170808",
                            "etc170808"
                        };
                        foreach (string saveName in saveNames)
                        {
                            if (adjustedFileName.Contains(saveName))
                            {
                                adjustedFileName = adjustedFileName.Replace(saveName + ".xml", "");
                                fileExists = true;
                                foreach (string saveName2 in saveNames)
                                {
                                    if (File.Exists(adjustedFileName + saveName2 + ".dat"))
                                    {
                                        SaveUtil.WriteSerializableFile(adjustedFileName + saveName2 + ".dat", SaveUtil.ReadSerializableFile(adjustedFileName + saveName2 + ".dat"));
                                    }
                                    else
                                    {
                                        UnityEngine.Debug.Log("doesn't exist " + adjustedFileName + saveName2 + ".dat");
                                    }
                                }
                                fileExists = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { UnityEngine.Debug.Log(ex.ToString()); }
        }
        public static void RecursiveOut<T, K>(Dictionary<T, K> x, int depth)
        {
            string indent = "";
            for (int i = depth; i > 0; i--)
            {
                indent += "  ";
            }
            foreach (var y in x)
            {
                if (y.Value.GetType().IsGenericType && Equals(y.Value.GetType().GetGenericTypeDefinition(), typeof(Dictionary<,>)))
                {
                    File.AppendAllText("out.txt", indent + "in " + y.Key.ToString() + "\n");
                    Type[] typesofz = {
                        y.Value.GetType()
                    };
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveOut));
                    MethodInfo generic = method.MakeGenericMethod(y.Value.GetType().GetGenericArguments());
                    object[] param = {
                        y.Value,
                        depth + 1
                    };
                    generic.Invoke(null, param);
                }
                else if (y.Value.GetType().IsGenericType && Equals(y.Value.GetType().GetGenericTypeDefinition(), typeof(List<>)))
                {

                    File.AppendAllText("out.txt", indent + "in " + y.Key.ToString() + "\n");
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.ListOut));
                    MethodInfo generic = method.MakeGenericMethod(y.Value.GetType().GetGenericArguments());
                    object[] param = {
                        y.Value,
                        depth + 1
                    };
                    generic.Invoke(null, param);
                }
                else
                {
                    File.AppendAllText("out.txt", indent + y.Key.ToString() + " is " + y.Value.ToString() + "\n");
                }
            }
        }
        public static void ListOut<T>(List<T> y, int depth)
        {
            if (y.Count == 0) { return; }
            string indent = "";
            for (int i = depth; i > 0; i--)
            {
                indent += "  ";
            }
            if (y[0].GetType().IsGenericType && Equals(y[0].GetType().GetGenericTypeDefinition(), typeof(Dictionary<,>)))
            {
                File.AppendAllText("out.txt", indent + "[");
                foreach (var z in y)
                {
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveOut));
                    MethodInfo generic = method.MakeGenericMethod(z.GetType().GetGenericArguments());
                    object[] param = {
                        z,
                        depth + 1
                    };
                    generic.Invoke(null, param);
                    File.AppendAllText("out.txt", indent + ",\n");
                }
                File.AppendAllText("out.txt", indent + "]\n");
            }
            else
            {
                File.AppendAllText("out.txt", indent + "[");
                foreach (var z in y)
                {
                    File.AppendAllText("out.txt", indent + z.ToString() + ",\n");
                }
                File.AppendAllText("out.txt", indent + "]\n");
            }
        }
        public static object RecursiveConvert<T, K>(Dictionary<T, K> x)
        {
            SerializableDictionary<T, object> convertdict = new SerializableDictionary<T, object>();
            foreach (var y in x)
            {
                if (y.Value.GetType().IsGenericType && Equals(y.Value.GetType().GetGenericTypeDefinition(), typeof(Dictionary<,>)))
                {
                    Type[] typesofz = {
                        y.Value.GetType()
                    };
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveConvert));
                    MethodInfo generic = method.MakeGenericMethod(y.Value.GetType().GetGenericArguments());
                    object[] param = {
                        y.Value
                    };
                    convertdict.Add(y.Key, generic.Invoke(null, param));

                }
                else if (y.Value.GetType().IsGenericType && Equals(y.Value.GetType().GetGenericTypeDefinition(), typeof(List<>)))
                {

                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.ListConvert));
                    MethodInfo generic = method.MakeGenericMethod(y.Value.GetType().GetGenericArguments());
                    object[] param = {
                        y.Value
                    };
                    //convertdict.Add(y.Key, generic.Invoke(null, param));
                }
                else
                {
                    convertdict.Add(y.Key, y.Value);
                }

            }
            return convertdict;
        }
        public static List<object> ListConvert<T>(List<T> y)
        {
            List<object> newlist = new List<object>();
            int index = 0;
            if (y.Count == 0) { return newlist; }
            if (y[0].GetType().IsGenericType && Equals(y[0].GetType().GetGenericTypeDefinition(), typeof(Dictionary<,>)))
            {
                foreach (var z in y)
                {
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveConvert));
                    MethodInfo generic = method.MakeGenericMethod(z.GetType().GetGenericArguments());
                    object[] param = {
                        z
                    };
                    newlist.Add(generic.Invoke(null, param));
                    index++;
                }
            }
            else
            {
                foreach (var z in y)
                {
                    newlist.Add(z);
                }
            }
            return newlist;
        }
        public static void RecursiveXml<T, K>(Dictionary<T, K> x, XmlElement superElement)
        {
            XmlElement thisElement = null;
            XmlElement testElement = null;
            foreach (var y in x)
            {
                if (y.Value.GetType().IsGenericType && Equals(y.Value.GetType().GetGenericTypeDefinition(), typeof(Dictionary<,>)))
                {
                    // make an element with the name of the dictionary
                    try
                    {
                        thisElement = save.CreateElement(y.Key.ToString());
                    }
                    catch (Exception ex)
                    {
                        // this is horrible practice
                        thisElement = save.CreateElement("num" + y.Key.ToString());
                    }
                    // store its type
                    testElement = save.CreateElement("type");
                    XmlText textToAdd = save.CreateTextNode(y.Value.GetType().ToString());
                    testElement.AppendChild(textToAdd);
                    thisElement.AppendChild(testElement);
                    // call this function on the dictionary (recursion!)
                    Type[] typesofz = {
                        y.Value.GetType(),
                        typeof(XmlElement)
                    };
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveXml));
                    MethodInfo generic = method.MakeGenericMethod(y.Value.GetType().GetGenericArguments());
                    object[] param = {
                        y.Value,
                        thisElement
                    };
                    testElement = (XmlElement)generic.Invoke(null, param);
                    // add the result as a child to this node
                    if (testElement != null)
                    {
                        thisElement.AppendChild(testElement);
                    }
                }
                else if (y.Value.GetType().IsGenericType && Equals(y.Value.GetType().GetGenericTypeDefinition(), typeof(List<>)))
                {
                    // make an element with the name of this list
                    thisElement = save.CreateElement(y.Key.ToString());
                    // most of the logic is in this funtion
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveListXml));
                    MethodInfo generic = method.MakeGenericMethod(y.Value.GetType().GetGenericArguments());
                    //LobotomyBaseMod.ModDebug.Log(y.Value.GetType().GetGenericArguments().ToString());
                    object[] param = {
                        y.Value,
                        thisElement
                    };
                    thisElement = (XmlElement)generic.Invoke(null, param);
                }
                else if (!y.Value.GetType().IsPrimitive && !y.Value.GetType().Equals(typeof(Type[])) && !y.Value.GetType().Equals(typeof(string)))
                {
                    // create an element with the name of the key
                    try
                    {
                        thisElement = save.CreateElement(y.Key.ToString());
                    }
                    catch (Exception ex)
                    {
                        // this is horrible practice
                        thisElement = save.CreateElement("num" + y.Key.ToString());
                    }
                    MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveObjectSerializer));
                    //LobotomyBaseMod.ModDebug.Log(y.GetType().GetGenericArguments().ToString());
                    MethodInfo generic = method.MakeGenericMethod(y.Value.GetType());
                    object[] param = {
                        y.Value,
                        thisElement
                    };
                    thisElement = (XmlElement)generic.Invoke(null, param);
                }
                else if (y.GetType().IsGenericType && Equals(y.GetType().GetGenericTypeDefinition(), typeof(KeyValuePair<,>)))
                {
                    try
                    {
                        thisElement = save.CreateElement(y.Key.ToString());
                    }
                    catch (Exception ex)
                    {
                        // this is horrible practice
                        thisElement = save.CreateElement("num" + y.Key.ToString());
                    }
                    testElement = save.CreateElement("type");
                    testElement.AppendChild(save.CreateTextNode(y.Value.GetType().ToString()));
                    thisElement.AppendChild(testElement);
                    testElement = save.CreateElement("value");
                    testElement.AppendChild(save.CreateTextNode(y.Value.ToString()));
                    thisElement.AppendChild(testElement);
                }
                else
                {
                    try
                    {
                        thisElement = save.CreateElement(y.Key.ToString());
                    }
                    catch (Exception ex)
                    {
                        // this is horrible practice
                        thisElement = save.CreateElement("num" + y.Key.ToString());
                    }
                    thisElement.AppendChild(save.CreateTextNode(y.Value.ToString()));
                }
                // add the element to the document
                superElement.AppendChild(thisElement);
            }
        }
        public static XmlElement RecursiveListXml<T>(List<T> y, XmlElement thisElement)
        {
            //LobotomyBaseMod.ModDebug.Log(y.ToString() + y.GetType().ToString());
            // make an element named List with the text of the list's type
            XmlElement testElement = null;
            testElement = save.CreateElement("type");
            testElement.AppendChild(save.CreateTextNode(y.GetType().ToString()));
            thisElement.AppendChild(testElement);
            int index = 0;
            if (y.Count != 0)
            {
                if (y[0].GetType().IsGenericType && Equals(y[0].GetType().GetGenericTypeDefinition(), typeof(Dictionary<,>)))
                {
                    foreach (var z in y)
                    {
                        XmlElement dictIndex = save.CreateElement("index" + index);
                        // run the recursive function on every dictionary in the list
                        MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveXml));
                        MethodInfo generic = method.MakeGenericMethod(z.GetType().GetGenericArguments());
                        object[] param = {
                            z,
                            dictIndex
                        };
                        // put each in an element whose name is the index of the dictionary in the list
                        generic.Invoke(null, param);
                        thisElement.AppendChild(dictIndex);
                        index++;
                    }
                }
                else if (!y[0].GetType().IsPrimitive && !y[0].GetType().Equals(typeof(string)))
                {
                    foreach (var z in y)
                    {
                        XmlElement subElement = save.CreateElement("index" + index);
                        MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveObjectSerializer));
                        //LobotomyBaseMod.ModDebug.Log(y.GetType().GetGenericArguments().ToString());
                        MethodInfo generic = method.MakeGenericMethod(y.GetType().GetGenericArguments());
                        object[] param = {
                        z,
                        subElement
                        };
                        thisElement.AppendChild((XmlElement)generic.Invoke(null, param));
                        index++;
                    }
                }
                else
                {
                    foreach (var z in y)
                    {
                        // put each value in the list in an element whose name is the index of it in the list
                        testElement = save.CreateElement("index" + index);
                        testElement.AppendChild(save.CreateTextNode(z.ToString()));
                        thisElement.AppendChild(testElement);
                        index++;
                    }
                }
            }
            // return the finished element to the caller
            return thisElement;
        }
        public static XmlElement RecursiveObjectSerializer<T>(T y, XmlElement thisElement)
        {
            XmlElement testElement;
            testElement = save.CreateElement("type");
            testElement.AppendChild(save.CreateTextNode(y.GetType().ToString()));
            thisElement.AppendChild(testElement);
            // serialize each field of the object to elements
            foreach (FieldInfo field in y.GetType().GetFields())
            {
                XmlElement objElement = save.CreateElement(field.Name);
                var z = field.GetValue(y);
                //LobotomyBaseMod.ModDebug.Log(field.FieldType.ToString());
                testElement = save.CreateElement("type");
                testElement.AppendChild(save.CreateTextNode(field.FieldType.ToString()));
                objElement.AppendChild(testElement);
                testElement = save.CreateElement("value");

                if (z != null)
                {
                    // if it's another object, recurse!
                    if (!field.FieldType.IsPrimitive && !field.FieldType.Equals(typeof(string)))
                    {
                        XmlElement subElement = save.CreateElement(field.Name);
                        MethodInfo method = typeof(Harmony_Patch).GetMethod(nameof(Harmony_Patch.RecursiveObjectSerializer));
                        MethodInfo generic = method.MakeGenericMethod(z.GetType());
                        object[] param = {
                        z,
                        testElement
                        };
                        objElement.AppendChild((XmlElement)generic.Invoke(null, param));
                    }
                    // otherwise just add the value
                    else
                    {
                        testElement.AppendChild(save.CreateTextNode(z.ToString()));
                    }
                }
                objElement.AppendChild(testElement);
                thisElement.AppendChild(objElement);
            }
            // return the finished element to the caller
            return thisElement;
        }

        public static Dictionary<string, object> DeserializeXml(string xmlName)
        {
            if (gameAssembly is null)
            {
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a.FullName.Contains("Assembly-CSharp"))
                    {
                        gameAssembly = a;
                        break;
                    }

                }
            }
            XmlDocument save = new XmlDocument();
            // this is a suprisingly significant optimization
            if (typeBuffer is null)
            {
                typeBuffer = new Dictionary<string, Type>();
                typeBuffer[typeof(string).ToString()] = typeof(string);
                typeBuffer[typeof(int).ToString()] = typeof(int);
                typeBuffer[typeof(long).ToString()] = typeof(long);
                typeBuffer[typeof(acs::RwbpType).ToString()] = typeof(acs::RwbpType);
            }
            save.Load(xmlName);
            Dictionary<string, object> baseDict = new Dictionary<string, object>();
            foreach (XmlNode node in save.FirstChild.ChildNodes)
            {
                baseDict.Add(node.Name, __DeserializeXml(node));
            }
            return baseDict;
            object __DeserializeXml(XmlNode node, Type objType = null)
            {
                bool shouldInvoke = true;
                string nodeName;
                object serializedThing = null;
                object thisDictionary = null;
                object thisList = null;
                Type[] dictTypes = null;

                if (node.Name.StartsWith("num"))
                {
                    nodeName = node.Name.Remove(0, 3);
                }
                else { nodeName = node.Name; }
                if (node.HasChildNodes && node.FirstChild.Name == "type")
                {
                    if (objType is null)
                    {
                        typeBuffer.TryGetValue(node.FirstChild.InnerText, out objType);
                    }
                    if (objType is null)
                    {
                        objType = Type.GetType(node.FirstChild.InnerText);
                    }
                    else
                    {
                        //LobotomyBaseMod.ModDebug.Log(objType.ToString());
                    }
                    // some types aren't so easy to find...
                    if (objType is null)
                    {
                        if (node.FirstChild.InnerText.StartsWith("System.Collections.Generic.Dictionary`2"))
                        {
                            dictTypes = MakeDict(node);
                            if (dictTypes[0] is null)
                            {
                                UnityEngine.Debug.Log("Can't find type of " + node.FirstChild.InnerText);
                            }
                            Type[] dictTypes2 = new Type[2];
                            dictTypes2[0] = dictTypes[0];
                            dictTypes2[1] = dictTypes[1];
                            MethodInfo method = typeof(Harmony_Patch).GetMethod("ActuallyMakeDict");
                            MethodInfo generic = method.MakeGenericMethod(dictTypes2);
                            thisDictionary = generic.Invoke(null, null);
                            typeBuffer[thisDictionary.GetType().ToString()] = thisDictionary.GetType();
                            shouldInvoke = false;
                            objType = thisDictionary.GetType();
                        }
                        else if (node.FirstChild.InnerText.StartsWith("System.Collections.Generic.List`1"))
                        {
                            Type listType = MakeList(node);
                            MethodInfo method = typeof(Harmony_Patch).GetMethod("ActuallyMakeList");
                            MethodInfo generic = method.MakeGenericMethod(new Type[] { listType });
                            thisList = generic.Invoke(null, null);
                            typeBuffer[thisList.GetType().ToString()] = thisList.GetType();
                            shouldInvoke = false;
                            objType = thisList.GetType();
                        }
                        else
                        {
                            string typestring = node.FirstChild.InnerText;
                            foreach (Type t in gameAssembly.GetTypes())
                            {
                                if (typestring.Equals(t.ToString()))
                                {
                                    // store it in the buffer for optimization purposes
                                    typeBuffer[t.ToString()] = t;
                                    objType = t;
                                    break;
                                    //LobotomyBaseMod.ModDebug.Log(t.ToString());
                                }
                            }
                            if (objType is null)
                            {
                                objType = GetTypeFromString(typestring);
                            }
                        }
                    }
                    else
                    {
                        if (!(objType is null) && objType.IsGenericType && Equals(objType.GetGenericTypeDefinition(), typeof(Dictionary<,>)))
                        {
                            if (!node.FirstChild.InnerText.Contains("Dictionary"))
                            {
                                dictTypes = objType.GetGenericArguments();
                            }
                            else
                            {
                                dictTypes = MakeDict(node);
                            }
                            Type[] dictTypes2;
                            MethodInfo method;
                            if (dictTypes.Length == 2 || dictTypes[2] is null)
                            {
                                method = typeof(Harmony_Patch).GetMethod("ActuallyMakeDict");
                                dictTypes2 = new Type[2];
                                dictTypes2[0] = dictTypes[0];
                                dictTypes2[1] = dictTypes[1];
                            }
                            else if (dictTypes[3] is null)
                            {
                                method = typeof(Harmony_Patch).GetMethod("ActuallyMakeDict2");
                                dictTypes2 = new Type[3];
                                dictTypes2[0] = dictTypes[0];
                                dictTypes2[1] = dictTypes[1];
                                dictTypes2[2] = dictTypes[2];
                            }
                            else
                            {
                                method = typeof(Harmony_Patch).GetMethod("ActuallyMakeDict3");
                                dictTypes2 = dictTypes;
                            }
                            MethodInfo generic = method.MakeGenericMethod(dictTypes2);
                            thisDictionary = generic.Invoke(null, null);
                        }
                        else if (!(objType is null) && objType.IsGenericType && Equals(objType.GetGenericTypeDefinition(), typeof(List<>)))
                        {
                            Type listType = MakeList(node);
                            MethodInfo method = typeof(Harmony_Patch).GetMethod("ActuallyMakeList");
                            MethodInfo generic = method.MakeGenericMethod(new Type[] { listType });
                            thisList = generic.Invoke(null, null);
                            typeBuffer[thisList.GetType().ToString()] = thisList.GetType();
                            shouldInvoke = false;
                            objType = thisList.GetType();
                        }
                    }
                }
                if (objType.IsGenericType && Equals(objType.GetGenericTypeDefinition(), typeof(Dictionary<,>)))
                {
                    foreach (XmlNode subNode in node.ChildNodes)
                    {
                        if (thisDictionary is null)
                        {
                            MethodInfo method = typeof(Harmony_Patch).GetMethod("ActuallyMakeDict");
                            MethodInfo generic = method.MakeGenericMethod(objType.GetGenericArguments());
                            thisDictionary = generic.Invoke(null, null);
                        }
                        if (subNode.Name != "type")
                        {
                            //LobotomyBaseMod.ModDebug.Log(subNode.Name + objType.GetGenericArguments()[0].ToString());
                            string subNodeName;
                            if (subNode.Name.StartsWith("num"))
                            {
                                subNodeName = subNode.Name.Remove(0, 3);
                            }
                            else { subNodeName = subNode.Name; }
                            MethodInfo method = typeof(Harmony_Patch).GetMethod("AddToDict");
                            MethodInfo generic = method.MakeGenericMethod(thisDictionary.GetType().GetGenericArguments());
                            object paramValue;
                            if (thisDictionary.GetType().GetGenericArguments()[0].IsEnum)
                            {
                                paramValue = Enum.Parse(thisDictionary.GetType().GetGenericArguments()[0], subNodeName);
                                //LobotomyBaseMod.ModDebug.Log("enum" + paramValue.ToString());
                            }
                            else
                            {
                                paramValue = Convert.ChangeType(subNodeName, thisDictionary.GetType().GetGenericArguments()[0]);
                            }
                            object[] parameters = {
                                    thisDictionary,
                                    paramValue,
                                    __DeserializeXml(subNode)
                                    };
                            generic.Invoke(null, parameters);
                        }
                    }
                    return thisDictionary;
                }
                if (objType.IsGenericType && Equals(objType.GetGenericTypeDefinition(), typeof(List<>)))
                {
                    foreach (XmlNode subNode in node.ChildNodes)
                    {
                        if (subNode.Name != "type")
                        {
                            MethodInfo method = typeof(Harmony_Patch).GetMethod("AddToList");
                            MethodInfo generic = method.MakeGenericMethod(objType.GetGenericArguments());
                            if (objType.GetGenericArguments() is null)
                            {
                                //LobotomyBaseMod.ModDebug.Log("Error!");
                            }
                            Type subNodeType = objType.GetGenericArguments()[0];
                            //LobotomyBaseMod.ModDebug.Log(objType.ToString());
                            //LobotomyBaseMod.ModDebug.Log(subNodeType.ToString());
                            object listval = __DeserializeXml(subNode, subNodeType);
                            object[] parameters = {
                                thisList,
                                listval
                                };
                            if (!(listval is null))
                            {
                                generic.Invoke(null, parameters);
                            }
                        }
                    }
                    return thisList;
                }
                else if (!objType.IsPrimitive && !objType.Equals(typeof(string)))
                {
                    MethodInfo method = typeof(Harmony_Patch).GetMethod("MakeObject");
                    MethodInfo generic = method.MakeGenericMethod(new Type[] { objType });
                    serializedThing = generic.Invoke(null, null);
                    if (node.LastChild.Name.Equals("value"))
                    {
                        node = node.LastChild;
                    }
                    foreach (FieldInfo field in objType.GetFields())
                    {
                        foreach (XmlNode subNode in node.ChildNodes)
                        {
                            string subNodeName;
                            if (subNode.Name.StartsWith("num"))
                            {
                                subNodeName = subNode.Name.Remove(0, 3);
                            }
                            else { subNodeName = subNode.Name; }
                            if (subNodeName == field.Name)
                            {
                                field.SetValue(serializedThing, __DeserializeXml(subNode));
                            }
                        }
                    }
                    return serializedThing;
                }
                else if (objType.Equals(typeof(string)))
                {
                    if (node.HasChildNodes)
                    {
                        return node.LastChild.InnerText;
                    }
                    return node.InnerText;
                }
                else
                {
                    if (Convert.ChangeType(node.LastChild.InnerText, objType) is null)
                        throw new Exception();
                    return Convert.ChangeType(node.LastChild.InnerText, objType);
                }
            }
            // I hope I make this less horrible later
            Type[] MakeDict(XmlNode node)
            {
                //LobotomyBaseMod.ModDebug.Log(node.Name);
                Type[] dictTypes = { null, null, null, null };
                string[] typeStrings = new string[6];
                string text = node.FirstChild.InnerText;
                text = text.Remove(0, 40);
                text = text.Replace("]", "");
                typeStrings = text.Split(',');
                dictTypes[0] = Type.GetType(typeStrings[0]);
                dictTypes[1] = Type.GetType(typeStrings[1]);
                if (typeStrings[1].Contains("Dictionary") && dictTypes[1] is null)
                {
                    typeStrings[1] = typeStrings[1].Remove(0, 40);
                    dictTypes[1] = Type.GetType(typeStrings[1]);
                    if (typeStrings[2].Contains("Dictionary"))
                    {
                        typeStrings[2] = typeStrings[2].Remove(0, 40);
                        dictTypes[2] = Type.GetType(typeStrings[2]);
                        dictTypes[3] = Type.GetType(typeStrings[3]);
                    }
                    else
                    {
                        dictTypes[2] = Type.GetType(typeStrings[2]);
                    }
                }
                if (dictTypes[0] is null)
                {
                    typeBuffer.TryGetValue(typeStrings[0], out dictTypes[0]);
                }
                if (dictTypes[0] is null)
                {
                    foreach (Type t in gameAssembly.GetTypes())
                    {
                        if (typeStrings[0].Equals(t.ToString()))
                        {
                            // store it in the buffer for optimization purposes
                            typeBuffer[t.ToString()] = t;
                            dictTypes[0] = t;
                            //LobotomyBaseMod.ModDebug.Log(t.ToString());
                        }
                    }

                    if (dictTypes[0] is null)
                    {
                        dictTypes[0] = GetTypeFromString(typeStrings[0]);
                    }
                }
                if (dictTypes[1] is null)
                {
                    typeBuffer.TryGetValue(typeStrings[1], out dictTypes[1]);
                }
                if (dictTypes[1] is null)
                {
                    foreach (Type t in gameAssembly.GetTypes())
                    {
                        if (typeStrings[0].Equals(t.ToString()))
                        {
                            // store it in the buffer for optimization purposes
                            typeBuffer[t.ToString()] = t;
                            dictTypes[1] = t;
                            //LobotomyBaseMod.ModDebug.Log(t.ToString());
                        }
                    }

                    if (dictTypes[1] is null)
                    {
                        dictTypes[1] = GetTypeFromString(typeStrings[1]);
                    }
                }
                return dictTypes;

            }
        }
        public static Dictionary<T, K> ActuallyMakeDict<T, K>()
        {
            return new Dictionary<T, K>();
        }
        public static Dictionary<T, Dictionary<K, L>> ActuallyMakeDict2<T, K, L>()
        {
            return new Dictionary<T, Dictionary<K, L>>();
        }
        public static Dictionary<T, Dictionary<K, Dictionary<L, M>>> ActuallyMakeDict3<T, K, L, M>()
        {
            return new Dictionary<T, Dictionary<K, Dictionary<L, M>>>();
        }
        public static void AddToDict<T, K>(IDictionary<T, K> theDict, T key, K value)
        {
            theDict.Add(key, value);
        }
        public static void AddToList<T>(IList<T> theList, T value)
        {
            theList.Add(value);
        }
        public static T MakeObject<T>() where T : new()
        {
            return new T();
        }
        public static T MakePrimitive<T>(object value)
        {

            return (T)value;
        }
        public static Type MakeList(XmlNode node)
        {
            Type listType = null;
            string text = node.FirstChild.InnerText;
            text = text.Remove(0, 34);
            text = text.Remove(text.Length - 1, 1);
            typeBuffer.TryGetValue(text, out listType);
            if (listType is null)
            {
                if (text.Contains("Dictionary"))
                {
                    //LobotomyBaseMod.ModDebug.Log("text=" + text);
                    Type[] dictTypes = { null, null };
                    string[] typeStrings;
                    text = text.Remove(0, 40);
                    text = text.Replace("]", "");
                    typeStrings = text.Split(',');
                    dictTypes[0] = Type.GetType(typeStrings[0]);
                    dictTypes[1] = Type.GetType(typeStrings[1]);
                    MethodInfo method = typeof(Harmony_Patch).GetMethod("ActuallyMakeDict");
                    MethodInfo generic = method.MakeGenericMethod(dictTypes);
                    return generic.Invoke(null, null).GetType();
                }
                listType = GetTypeFromString(text);
            }
            return listType;
        }
        public static List<T> ActuallyMakeList<T>()
        {
            return new List<T>();
        }
        public static Type GetTypeFromString(string typeString)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] typeList;
                try
                {
                    typeList = a.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    ModDebug.Log("SaveAsXml: Type(s) from assembly " + a.FullName + " failed to load! Skipping it and continuing");
                    continue;
                }
                foreach (Type t in typeList)
                {
                    if (typeString == t.ToString())
                    {
                        // store it in the buffer for optimization purposes
                        typeBuffer[t.ToString()] = t;
                        return t;
                    }
                }
            }
            ModDebug.Log("SaveAsXml: Couldn't find type " + typeString);
            return null;
        }
    } // end class
    // unused
    [XmlRoot("dictionary")]

    public class SerializableDictionary<TKey, TValue>

    : Dictionary<TKey, TValue>, IXmlSerializable

    {

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()

        {

            return null;

        }



        public void ReadXml(System.Xml.XmlReader reader)

        {

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));



            bool wasEmpty = reader.IsEmptyElement;

            reader.Read();



            if (wasEmpty)

                return;



            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)

            {

                reader.ReadStartElement("item");



                reader.ReadStartElement("key");

                TKey key = (TKey)keySerializer.Deserialize(reader);

                reader.ReadEndElement();



                reader.ReadStartElement("value");

                TValue value = (TValue)valueSerializer.Deserialize(reader);

                reader.ReadEndElement();



                this.Add(key, value);



                reader.ReadEndElement();

                reader.MoveToContent();

            }

            reader.ReadEndElement();

        }



        public void WriteXml(System.Xml.XmlWriter writer)

        {

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));



            foreach (TKey key in this.Keys)

            {

                writer.WriteStartElement("item");



                writer.WriteStartElement("key");

                keySerializer.Serialize(writer, key);

                writer.WriteEndElement();



                writer.WriteStartElement("value");

                TValue value = this[key];

                valueSerializer.Serialize(writer, value);

                writer.WriteEndElement();



                writer.WriteEndElement();

            }

        }

        #endregion

    }

} // end namespace
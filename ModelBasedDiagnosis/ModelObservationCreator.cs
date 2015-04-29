using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModelBasedDiagnosis
{
    class ModelObservationCreator
    {
        public ModelObservationCreator()
        {
        }
     
        public Dictionary<int, List<int>> ReadRealObsFiles(string fileName)
        {
            Dictionary<int, List<int>> ans = new Dictionary<int, List<int>>();
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs);
            string model_allText = reader.ReadToEnd();
            fs.Close();
            reader.Close();
            fs = null;
            reader = null;

            char[] delrow = new char[2];
            delrow[0] = '\n';
            delrow[1] = '\r';
            List<string> rows = model_allText.Split(delrow, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (rows == null || rows.Count == 0)
                return null;

            foreach (string row in rows)
            {
                char[] del = new char[2];
                del[0] = ':';
                del[1] = ',';
                List<string> obReal = row.Split(del, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (obReal == null || obReal.Count == 0)
                    continue;
                int obId = 0;
                if (!Int32.TryParse(obReal[0], out obId) || ans.ContainsKey(obId))
                    continue;
                //ans.Add(obId, new List<int>());
                List<int> list = new List<int>();
                bool addObs = true;
                for (int i = 1; i < obReal.Count; i++)
                {
                    int ComponentID = 0;
                    if (!Int32.TryParse(obReal[i], out ComponentID))
                    {
                        addObs = false;
                        break;
                    }
                    list.Add(ComponentID);
                }
                if(addObs)
                    ans.Add(obId, list);
            }

            return ans;
        }
        public List<Observation> ReadObsModelFiles(string fileModel, string fileObs) //path?
        {
            //reading model file
            List<Observation> observationsList = new List<Observation>();
           // if (observationsList.Count > 0)
             //   observationsList.Clear();
            FileStream fs = new FileStream(fileModel, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs);
            string model_allText = reader.ReadToEnd();
            fs.Close();
            reader.Close();
            fs = null;
            reader = null;

            char[] delrow = new char[2];
            delrow[0] = '\n';
            delrow[1] = '\r';
            List<string> rows = model_allText.Split(delrow, StringSplitOptions.RemoveEmptyEntries).ToList();
            if(rows==null || rows.Count<4)
                return null; // throw

            //build Model
            SystemModel theModel;
            string modelID = rows[0].Substring(0, rows[0].Length - 1);
            List<Wire> inputs = new List<Wire>();
            List<Wire> outputs = new List<Wire>();
            List<Wire> internalWires = new List<Wire>();
            Dictionary<Wire.WireType, Dictionary<int, Wire>> wiresDictionary = new Dictionary<Wire.WireType, Dictionary<int, Wire>>();
            wiresDictionary.Add(Wire.WireType.i, new Dictionary<int, Wire>());
            wiresDictionary.Add(Wire.WireType.o, new Dictionary<int, Wire>());
            wiresDictionary.Add(Wire.WireType.z, new Dictionary<int, Wire>());
            //model id
           // if (!Int32.TryParse(rows[0].Substring(0, rows[0].Length - 1), out modelID))
             //   return; //throw

            char[] del = new char[6];
            del[0]='.';
            del[1]=',';
            del[2]='[';
            del[3]=']';
            del[4]='(';
            del[5]=')';

            //Wire.WiresDictionary = new Dictionary<string, Wire>(); 

            //input & output
            string[] inputArr = rows[1].Split(del, StringSplitOptions.RemoveEmptyEntries);
            string[] outputArr = rows[2].Split(del, StringSplitOptions.RemoveEmptyEntries);


            for (int i=0; i < inputArr.Length; i++)
            {
                //need to check if the Value is Valid? 2<=len<=3, starts with i/o/z, end with a number
                //need to check if theres as similar wire exist
                string wireName = inputArr[i];
                int wid; 
                if(Int32.TryParse(wireName.Substring(1),out wid))
                {
                    if (wireName.StartsWith("i"))
                    {
                        Wire w = new Wire(wid, Wire.WireType.i);
                        inputs.Add(w);
                        wiresDictionary[Wire.WireType.i].Add(wid, w);
                    }
                        
                }
                //else --
            }
            for (int j = 0; j < outputArr.Length; j++)
            {
                string wireName = outputArr[j];
                int wid;
                if (Int32.TryParse(wireName.Substring(1), out wid))
                {
                    if (wireName.StartsWith("o"))
                    {
                        Wire w = new Wire(wid, Wire.WireType.o);
                        outputs.Add(w);
                        wiresDictionary[Wire.WireType.o].Add(wid, w);
                    }
                        
                }
            }
            theModel = new SystemModel(modelID, inputs, outputs);

            //creating components
            for (int i = 3; i < rows.Count; i++)
            {
                if (!String.IsNullOrEmpty(rows[i])) 
                    theModel.AddComponent(CreateComponent(rows[i].Split(del, StringSplitOptions.RemoveEmptyEntries),theModel,wiresDictionary));
            }

            //sort model
            theModel.SortComponents();

            //reading observation fila
            delrow = new char[1];
            delrow[0] = '.';
            del = new char[7];
            del[0] = '\r';
            del[1] = ',';
            del[2] = '[';
            del[3] = ']';
            del[4] = '(';
            del[5] = ')';
            del[6] = '\n';
            rows = null;
            fs = new FileStream(fileObs, FileMode.Open, FileAccess.Read);
            reader = new StreamReader(fs);
            string ob_allText = reader.ReadToEnd();
            rows = ob_allText.Split(delrow, StringSplitOptions.RemoveEmptyEntries).ToList();
            fs.Close();
            reader.Close();
            fs = null;
            reader = null;

            if (rows == null || rows.Count == 0)
                return null; // throw


            //build observation
           // List<Observation> obList = new List<Observation>();
            for (int i = 0; i < rows.Count; i++)
            {
                string[] obArr = rows[i].Split(del, StringSplitOptions.RemoveEmptyEntries);
                if (obArr == null || obArr.Length == 0&& i!=rows.Count-1)
                    return null; //throw
                if (obArr.Length == 2 + inputs.Count + outputs.Count)
                {
                    if (!obArr[0].Equals(modelID))
                        return null; //throw
                    Observation toAdd = CreateObservation(obArr);
                    if (toAdd != null) 
                    {
                        toAdd.TheModel = theModel; //try catch
                        observationsList.Add(toAdd);
                    }
                }
                //else return/throw?
            }
            return observationsList;
        }

        private Gate CreateComponent(string[] compArr, SystemModel theModel, Dictionary<Wire.WireType, Dictionary<int, Wire>> wiresDictionary)
        {
            if(compArr==null||compArr.Length<4)
                return null; //throw

            int id;

            if(compArr[1].Length<5 || !compArr[1].StartsWith("Component"))
                return null; //throw

            if(!Int32.TryParse(compArr[1].Substring(4),out id))
                return null; //throw
            Wire output = null;
            string oname = compArr[2];
            int oid;
            if(Int32.TryParse(oname.Substring(1),out oid))
            {
                if (oname.StartsWith("o"))
                {
                   if(!wiresDictionary[Wire.WireType.o].TryGetValue(oid,out output))
                   {
                       output = new Wire(oid,Wire.WireType.o);
                       wiresDictionary[Wire.WireType.o].Add(oid, output);
                       theModel.Output.Add(output);
                   }

                }
                else if(oname.StartsWith("z"))
                {
                    if (!wiresDictionary[Wire.WireType.z].TryGetValue(oid, out output))
                    {
                        output = new Wire(oid, Wire.WireType.z);
                        wiresDictionary[Wire.WireType.z].Add(oid, output);
                        theModel.Internal.Add(output);
                    }
                }
                //cant be i 
            }
            if (compArr.Length == 4)
            {
                OneInputComponent ans = null;
                if (compArr[0].Equals("inverter"))
                    ans = new OneInputComponent(id, Gate.Type.not);
                if (compArr[0].Equals("buffer"))
                    ans = new OneInputComponent(id, Gate.Type.buffer);
                if (ans != null)
                {
                    if(output!=null)
                        ans.Output = output;
                    string iname = compArr[3];
                    int inid;
                    Wire input = null;
                    if (Int32.TryParse(iname.Substring(1), out inid))
                    {
                        if (iname.StartsWith("o"))
                        {
                            if (!wiresDictionary[Wire.WireType.o].TryGetValue(inid, out input))
                            {
                                input = new Wire(inid, Wire.WireType.o);
                                wiresDictionary[Wire.WireType.o].Add(inid, input);
                                theModel.Output.Add(input);
                            }

                        }
                        else if (iname.StartsWith("z"))
                        {
                            if (!wiresDictionary[Wire.WireType.z].TryGetValue(inid, out input))
                            {
                                input = new Wire(inid, Wire.WireType.z);
                                wiresDictionary[Wire.WireType.z].Add(inid, input);
                                theModel.Internal.Add(input);
                            }
                        }
                        else if (iname.StartsWith("i"))
                        {
                            if (!wiresDictionary[Wire.WireType.i].TryGetValue(inid, out input))
                            {
                                input = new Wire(inid, Wire.WireType.i);
                                wiresDictionary[Wire.WireType.i].Add(inid, input);
                                theModel.Input.Add(input);
                            }
                        }
                    }
                    if (input != null)
                        ans.Input1 = input;
                }
                return ans;
            }
            if (compArr.Length >= 5)
            {
                MultipleInputComponent ans = null;
                if (compArr[0].StartsWith("and"))
                    ans = new MultipleInputComponent(id, Gate.Type.and);
                if (compArr[0].StartsWith("nor"))
                    ans = new MultipleInputComponent(id, Gate.Type.nor);
                if (compArr[0].StartsWith("xor"))
                    ans = new MultipleInputComponent(id, Gate.Type.xor);
                if (compArr[0].StartsWith("nand"))
                    ans = new MultipleInputComponent(id, Gate.Type.nand);
                if (compArr[0].StartsWith("or"))
                    ans = new MultipleInputComponent(id, Gate.Type.or);
                if (ans != null)
                {
                    if (output != null)
                        ans.Output = output;
                    for (int i = 3; i < compArr.Length; i++)
                    {
                        string iname = compArr[i];
                        int inid;
                        Wire input = null;
                        if (Int32.TryParse(iname.Substring(1), out inid))
                        {
                            if (iname.StartsWith("o"))
                            {
                                if (!wiresDictionary[Wire.WireType.o].TryGetValue(inid, out input))
                                {
                                    input = new Wire(inid, Wire.WireType.o);
                                    wiresDictionary[Wire.WireType.o].Add(inid, input);
                                    theModel.Output.Add(input);
                                }

                            }
                            else if (iname.StartsWith("z"))
                            {
                                if (!wiresDictionary[Wire.WireType.z].TryGetValue(inid, out input))
                                {
                                    input = new Wire(inid, Wire.WireType.z);
                                    wiresDictionary[Wire.WireType.z].Add(inid, input);
                                    theModel.Internal.Add(input);
                                }
                            }
                            else if (iname.StartsWith("i"))
                            {
                                if (!wiresDictionary[Wire.WireType.i].TryGetValue(inid, out input))
                                {
                                    input = new Wire(inid, Wire.WireType.i);
                                    wiresDictionary[Wire.WireType.i].Add(inid, input);
                                    theModel.Input.Add(input);
                                }
                            }
                        }
                        if (input != null)
                            ans.AddInput(input); 
                    }
                }
                return ans;
            }
            return null;
        }

        private Observation CreateObservation(string[] obArr)
        {
            if (obArr == null || obArr.Length < 3)
                return null; //throw

            int id;
            if (!Int32.TryParse(obArr[1], out id))
                return null;// throw

            List<bool> inputVals = new List<bool>();
            List<bool> outputVals = new List<bool>();

            for (int i = 2; i < obArr.Length; i++)
            {
                if (obArr[i].Length > 4)
                    return null; //throw
                if (obArr[i].Contains('i')) //input
                {
                    if (obArr[i].StartsWith("-"))
                        inputVals.Add(false);
                    else
                        inputVals.Add(true);
                }
                else if (obArr[i].Contains('o')) //output
                {
                    if (obArr[i].StartsWith("-"))
                        outputVals.Add(false);
                    else
                        outputVals.Add(true);
                }
            }
            Observation ans = new Observation(id, inputVals.ToArray(), outputVals.ToArray());
            return ans;
        }

    }
}

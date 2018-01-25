using System.Collections.Generic;

public class JsonContainer
{

    public string JsonName;
    public string Version;
    public List<string> InfoObjects;

    public JsonContainer()
    {
        InfoObjects = new List<string>();
    }

}
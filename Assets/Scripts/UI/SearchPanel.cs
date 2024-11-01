using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoShared;
using MiniJSON;

public class SearchPanel : BasePanel
{

    private TrackPanel trackpanel = null;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        trackpanel = GameObject.FindFirstObjectByType<TrackPanel>(FindObjectsInactive.Include);

        var dd = GetComponentInChildren<Dropdown>();
        dd.options.Clear();
        for(int i=0;i<= (int)LocationManagerEnums.DemoLocation.HongKong;i++)
        {
            LocationManagerEnums.DemoLocation e = (LocationManagerEnums.DemoLocation)i;
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = e.ToString();
            dd.options.Add(od);
        }
    }

    public void OnSelectLocation()
    {
        var dd = GetComponentInChildren<Dropdown>();
        LocationManagerEnums.DemoLocation e = (LocationManagerEnums.DemoLocation)dd.value;
        Coordinates coord = LocationManagerEnums.LocationEnums.GetCoordinates(e);
        var inputfield = GetComponentInChildren<InputField>();
        inputfield.text = coord.toLongLatString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void OnSearch()
    {
        trackpanel.OnExitTrack();

        var inputfield = GetComponentInChildren<InputField>();
        if (inputfield!=null)
        {
            string str = await GetTextFromURL("https://api.maptiler.com/geocoding/" + inputfield.text + ".json?key=d6qPzaurptRX6n3oM6eX");

            if (str != null && str.Length>0)
            {
                var dict = Json.Deserialize(str) as Dictionary<string, object>;

                if (dict.ContainsKey("features"))
                {
                    var list = dict["features"] as List<object>;

                    if (list.Count > 0)
                    {
                        var subdict = list[0] as Dictionary<string, object>;
                        if (subdict.ContainsKey("center"))
                        {
                            var cc = subdict["center"] as List<object>;
                            if (cc.Count == 2)
                            {
                                double dlong = 0;
                                double dlat = 0;
                                bool ret1=double.TryParse(cc[0].ToString(), out dlong);
                                bool ret2=double.TryParse(cc[1].ToString(), out dlat);
                                if (ret1 && ret2 && !(dlong == 0 && dlat == 0))
                                {
                                    Coordinates coord = new Coordinates(dlat, dlong);
                                    //loc.motionMode = LocationManagerEnums.MotionMode.Avatar;
                                    //loc.SetLocation(coord);
                                }
                            }
                        }
                    }

                }

            }
        }
    }
}

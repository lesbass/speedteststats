﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Common;

namespace SpeedTestStats.BL
{
  public class StatsReader
  {
    private readonly Utility _utilityManager;

    public StatsReader()
    {
      _utilityManager = new Utility();
    }
    
    public List<StatRow> Get()
    {
      var filePath = @"\\192.168.1.254\Public\speed_log.txt";
      var content = File.ReadAllLines(filePath);
      var response = new List<StatRow>();

      var tempData = default(DateTime);
      decimal? tempPing = null;
      decimal? tempUpload = null;
      decimal? tempDownload = null;
      foreach (var item in content)
        try
        {
          if (item.StartsWith("--"))
          {
            // separatore
            response.Add(new StatRow
            {
              Ping = tempPing,
              DataOra = tempData,
              Upload = tempUpload,
              Download = tempDownload
            });

            tempPing = 0;
            tempData = default;
            tempUpload = 0;
            tempDownload = 0;
          }
          else if (item.StartsWith("Ping"))
          {
            // ping
            tempPing = _utilityManager.DecimalParse(item.Split(' ')[1]);
          }
          else if (item.StartsWith("Download"))
          {
            // download
            tempDownload = _utilityManager.DecimalParse(item.Split(' ')[1]);
          }
          else if (item.StartsWith("Upload"))
          {
            // download
            tempUpload = _utilityManager.DecimalParse(item.Split(' ')[1]);
          }
          else
          {
            var item1 = _utilityManager.CleanUrl(item).Split('-').Skip(1).ToList();
            item1 = item1.Take(item1.Count - 1).ToList();
            var item2 = string.Join("-", item1);
            // data dom 20 gen 2019, 00.20.01
            if (DateTime.TryParseExact(item2, @"d-MMM-yyyy-HH-mm-ss", CultureInfo.CurrentCulture,
              DateTimeStyles.None, out tempData))
            {
              //
            }
            //else if (DateTime.TryParseExact(item1, @"ddd-dd-MMM-yyyy-HH-mm-ss-CET", CultureInfo.CurrentCulture,
            //  DateTimeStyles.None, out tempData))
            //{
            //  //
            //}
            else
            {
              Console.WriteLine(item2);
            }
          }
        }
        catch (Exception e)
        {
          Console.WriteLine(item);
        }

      return response;
    }
  }
}
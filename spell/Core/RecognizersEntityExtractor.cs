using System;
using System.Collections.Generic;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.NumberWithUnit;


namespace spell.Core;


public class RecognizersEntityExtractor : IEntityExtractor
{
    private readonly string _culture = Culture.English;


    public void Extract(string input, IntentResult result)
    {
        try
        {
            var dt = DateTimeRecognizer.RecognizeDateTime(input, _culture);
            if (dt != null && dt.Count > 0)
            {
                result.Entities["datetime"] = dt;
            }
        }
        catch { }


        try
        {
            var nums = NumberRecognizer.RecognizeNumber(input, _culture);
            if (nums != null && nums.Count > 0)
            {
                result.Entities["number"] = nums;
            }
        }
        catch { }


        try
        {
            var recognizer = new NumberWithUnitRecognizer(_culture);
            var model = recognizer.GetDimensionModel(_culture);
            var units = model.Parse(input);

            if (units != null && units.Count > 0)
            {
                result.Entities["units"] = units;
            }
        }
        catch { }


        if (!result.Entities.ContainsKey("text") || result.Entities["text"] == null || string.IsNullOrWhiteSpace(result.Entities["text"].ToString()))
        {
            result.Entities["text"] = input;
        }

    }
}
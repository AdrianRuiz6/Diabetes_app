using System;

public class ActionLog
{
    private DateTime? _dateAndTime;
    private string _information;

    public ActionLog(DateTime? dateTime, string information)
    {
        this._dateAndTime = dateTime;
        this._information = information;
    }

    public DateTime? GetDateAndTime()
    {
        return _dateAndTime;
    }

    public string GetInformation()
    {
        return _information;
    }
}

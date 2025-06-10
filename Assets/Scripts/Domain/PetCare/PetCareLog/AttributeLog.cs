using System;

public class AttributeLog
{
    private DateTime? _dateAndTime;
    private int _value;

    public AttributeLog(DateTime? dateTime, int value)
    {
        this._dateAndTime = dateTime;
        this._value = value;
    }

    public DateTime? GetDateAndTime()
    {
        return _dateAndTime;
    }

    public int GetValue()
    {
        return _value;
    }
}

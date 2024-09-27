using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Graph : MonoBehaviour
{
    void Start()
    {
        
    }


















    //private void LoQueAntesEstabaEnElStart()
    //{
    //    DataStorage.SaveGlycemiaGraph(DateTime.Now, 150);
    //    DataStorage.SaveGlycemiaGraph(DateTime.Now, 160);

    //    List<AttributeData> attributeDataList = DataStorage.LoadGlycemiaGraph();
    //    foreach (AttributeData glycemiaData in attributeDataList)
    //    {
    //        DateTime dateAndTime = DateTime.Parse(glycemiaData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
    //        float glycemiaValue = glycemiaData.Value;

    //        Debug.Log($"Date and time: {dateAndTime}. Glycemia value: {glycemiaValue}.");
    //    }
    //}








    //// TODO: Borrar, es para saberlo para luego.
    //private void SepararYcompararFechaYHora()
    //{
    //    List<AttributeData> attributeDataList = DataStorage.LoadGlycemiaGraph();

    //    foreach (AttributeData glycemiaData in attributeDataList)
    //    {
    //        // Parseamos el DateTime desde el string almacenado en glycemiaData
    //        DateTime dateAndTime = DateTime.Parse(glycemiaData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);

    //        // Separar la fecha y la hora
    //        DateTime onlyDate = dateAndTime.Date; // Solo la fecha (hora 00:00:00)
    //        TimeSpan onlyTime = dateAndTime.TimeOfDay; // Solo la hora del día
    //        int hour = dateAndTime.Hour; // Solo la hora
    //        int minute = dateAndTime.Minute; // Solo el minuto

    //        // Imprimir ambos valores
    //        Debug.Log($"Fecha: {onlyDate.ToShortDateString()}. Hora: {onlyTime}.");

    //        // Ejemplo de comparación con otra fecha y hora
    //        DateTime compareDate = new DateTime(2024, 9, 22);
    //        TimeSpan compareTime = new TimeSpan(14, 30, 0); // 14:30:00

    //        // Comparación de fechas
    //        if (onlyDate == compareDate)
    //        {
    //            Debug.Log("Las fechas coinciden.");
    //        }

    //        // Comparación de horas
    //        if (onlyTime < compareTime)
    //        {
    //            Debug.Log("La hora es anterior a las 14:30.");
    //        }
    //        else
    //        {
    //            Debug.Log("La hora es posterior a las 14:30.");
    //        }

    //        // Obtener el valor de glucemia
    //        float glycemiaValue = glycemiaData.Value;
    //        Debug.Log($"Valor de glucemia: {glycemiaValue}.");
    //    }

    //}










    //// TODO: Borrar, es para saberlo para luego.
    //private void VerSiUnaFechaTieneMasDeNoSeCuantosDias()
    //{
    //    List<AttributeData> attributeDataList = DataStorage.LoadGlycemiaGraph();

    //    // Obtener la fecha actual
    //    DateTime currentDate = DateTime.Now;

    //    // Restar 10 días de la fecha actual
    //    DateTime thresholdDate = currentDate.AddDays(-10);

    //    foreach (AttributeData glycemiaData in attributeDataList)
    //    {
    //        // Parseamos la fecha guardada
    //        DateTime dateAndTime = DateTime.Parse(glycemiaData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);

    //        // Comparar si la fecha almacenada es anterior a hace 10 días
    //        if (dateAndTime < thresholdDate)
    //        {
    //            Debug.Log($"La fecha {dateAndTime.ToShortDateString()} es de hace más de 10 días.");
    //        }
    //        else
    //        {
    //            Debug.Log($"La fecha {dateAndTime.ToShortDateString()} es más reciente o igual a hace 10 días.");
    //        }

    //        // Obtener el valor de glucemia
    //        float glycemiaValue = glycemiaData.Value;
    //        Debug.Log($"Valor de glucemia: {glycemiaValue}.");
    //    }
    //}
}

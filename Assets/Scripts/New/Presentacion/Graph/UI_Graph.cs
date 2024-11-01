// Código basado en un tutorial de CodeMonkey
// Fuente: https://unitycodemonkey.com/video.php?v=CmU5-v-v1Qo
// Modificado para ajustarse a las necesidades del proyecto

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Graph : MonoBehaviour
{
    [SerializeField] private Sprite _circleSprite;
    [SerializeField] private RectTransform _graphContainer;

    private int _minHour = 0;
    private int _maxHour = 0;
    private Dictionary<DateTime, int> _insulinInfo = new Dictionary<DateTime, int>();
    private Dictionary<DateTime, int> _exerciseInfo = new Dictionary<DateTime, int>();
    private Dictionary<DateTime, int> _foodInfo = new Dictionary<DateTime, int>();
    private DateTime _currentDate = DateTime.Now;

    [Header("Prefabs")]
    [SerializeField] private GameObject _labelPrefab;
    [SerializeField] private GameObject _flashingLineXPrefab;
    [SerializeField] private GameObject _flashingLineYPrefab;

    private void Awake()
    {
        GameEventsGraph.OnInitialTimeModified += ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified += ModifyFinishHour;

        //GameEventsGraph.OnUpdatedGlycemiaGraph += LoadData;
        //GameEventsGraph.OnUpdatedActivityGraph += LoadData;
        //GameEventsGraph.OnUpdatedHungerGraph += LoadData;

        GameEventsGraph.OnUpdatedDateGraph += UpdateDate;
    }

    private void OnDestroy()
    {
        GameEventsGraph.OnInitialTimeModified -= ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified -= ModifyFinishHour;

        //GameEventsGraph.OnUpdatedGlycemiaGraph -= LoadData;
        //GameEventsGraph.OnUpdatedActivityGraph -= LoadData;
        //GameEventsGraph.OnUpdatedHungerGraph -= LoadData;

        GameEventsGraph.OnUpdatedDateGraph -= UpdateDate;
    }

    private void Start()
    {
        // Se establece el minimo y máximo de la franja horaria.
        ModifyInitialHour(LimitHours.Instance.initialTime.Hours);
        ModifyFinishHour(LimitHours.Instance.finishTime.Hours);
        
        // Iniciar datos fecha actual.
        UpdateDate(DateTime.Now);

        // TODO: quitar esto
        List<int> valueList = new List<int>() {250, 250, 56, 45, 30, 0, 0, 15, 13, 17, 25, 37, 40, 36, 33};
        ShowGraph(valueList);
    }

    private void UpdateDate(DateTime newCurrentDate)
    {
        _currentDate = newCurrentDate;
        //LoadData();

        // TODO: Limpiar y ponerle los nuevos labels.
        // TODO: Limpiar y poner los datos de los atributos de la nueva fecha.
    }

    // TODO: Carga los datos de los atributos.
    //private void LoadData()
    //{
    //    _insulinInfo.Clear();
    //    _exerciseInfo.Clear();
    //    _foodInfo.Clear();
    //    _availableTimes.Clear(); // TODO: quiza podriamos hacer una lista con las horas puntas

    //    _insulinInfo = DataStorage.LoadInsulinGraph(_currentDate);
    //    foreach (DateTime newDate in _insulinInfo.Keys)
    //    {
    //        if (!_availableTimes.Contains(newDate))
    //        {
    //            _availableTimes.Add(newDate);
    //        }
    //    }
    //    _exerciseInfo = DataStorage.LoadExerciseGraph(_currentDate);
    //    foreach (DateTime newDate in _exerciseInfo.Keys)
    //    {
    //        if (!_availableTimes.Contains(newDate))
    //        {
    //            _availableTimes.Add(newDate);
    //        }
    //    }
    //    _foodInfo = DataStorage.LoadFoodGraph(_currentDate);
    //    foreach (DateTime newDate in _foodInfo.Keys)
    //    {
    //        if (!_availableTimes.Contains(newDate))
    //        {
    //            _availableTimes.Add(newDate);
    //        }
    //    }
    //}

    private GameObject CreateCircle(Vector2 anchoredPosition) 
    {
        GameObject gameObjectCircle = new GameObject("circle", typeof(Image));
        // Asigna el padre del sprite.
        gameObjectCircle.transform.SetParent(_graphContainer, false);
        // Le asigna la imágen del círculo al sprite.
        gameObjectCircle.GetComponent<Image>().sprite = _circleSprite;
        // Configura el rectTransform del sprite.
        RectTransform rectTransformCircle = gameObjectCircle.GetComponent<RectTransform>();
        rectTransformCircle.anchoredPosition = anchoredPosition;
        rectTransformCircle.sizeDelta = new Vector2(0, 0);
        rectTransformCircle.anchorMin = Vector2.zero;
        rectTransformCircle.anchorMax = Vector2.zero;
        return gameObjectCircle;
    }

    private void ShowGraph(List<int> valueList)
    {
        float graphHeight = _graphContainer.sizeDelta.y; // Tamaño máximo de altura.
        float graphWidth = _graphContainer.sizeDelta.x;// Tamaño máximo de altura.
        float yMaximum = 250f; // Valor máximo en el eje vertical.
        float xSize = graphWidth / (valueList.Count - 1); // Distancia horizontal entre punto y punto en el eje horizontal.

        GameObject lastGameObjectCircle = null;
        for(int i = 0; i < valueList.Count; i++)
        {
            float xPosition = i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject currentGameObjectCircle = CreateCircle(new Vector2(xPosition, yPosition)); // Creación de un punto en la posición actual del gráfico.
            // Unión de un punto con el anterior.
            if (lastGameObjectCircle != null)
            {
                CreateDotConection(lastGameObjectCircle.GetComponent<RectTransform>().anchoredPosition, currentGameObjectCircle.GetComponent<RectTransform>().anchoredPosition);
            }
            lastGameObjectCircle = currentGameObjectCircle;

            // Colocación de la etiqueta de valor del eje horizontal.
            RectTransform label_X = Instantiate(_labelPrefab).GetComponent<RectTransform>();
            label_X.SetParent(_graphContainer, false);
            label_X.gameObject.SetActive(true);
            label_X.anchorMin = Vector2.zero;
            label_X.anchorMax = Vector2.zero;
            label_X.anchoredPosition = new Vector2(xPosition, -2.5f);
            label_X.GetComponent<TextMeshProUGUI>().text = "8:00";

            // Colocación de la linea intermitente de valor del eje horizontal.
            RectTransform flashingLine_X = Instantiate(_flashingLineXPrefab).GetComponent<RectTransform>();
            flashingLine_X.SetParent(_graphContainer, false);
            flashingLine_X.SetSiblingIndex(1);
            flashingLine_X.gameObject.SetActive(true);
            flashingLine_X.anchorMin = Vector2.zero;
            flashingLine_X.anchorMax = Vector2.zero;
            flashingLine_X.anchoredPosition = new Vector2(xPosition, -1.1f);
        }

        int separatorCount = 10; // Cantidad de valores que se van a representar en el eje.
        for (int i = 0; i <= separatorCount; i++)
        {
            // Colocación de la etiqueta de valor del eje vertical.
            RectTransform label_Y = Instantiate(_labelPrefab).GetComponent<RectTransform>();
            label_Y.SetParent(_graphContainer, false);
            label_Y.gameObject.SetActive(true);
            label_Y.anchorMin = Vector2.zero;
            label_Y.anchorMax = Vector2.zero;
            float normalizedValue = i * 1f / separatorCount;
            label_Y.anchoredPosition = new Vector2(-5.5f, normalizedValue * graphHeight);
            label_Y.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();

            RectTransform flashingLine_Y = Instantiate(_flashingLineYPrefab).GetComponent<RectTransform>();
            flashingLine_Y.SetParent(_graphContainer, false);
            flashingLine_Y.SetSiblingIndex(1);
            flashingLine_Y.gameObject.SetActive(true);
            flashingLine_Y.anchorMin = Vector2.zero;
            flashingLine_Y.anchorMax = Vector2.zero;
            flashingLine_Y.anchoredPosition = new Vector2(-2.1f, normalizedValue * graphHeight);
        }
    }

    private void CreateDotConection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObjectConnection = new GameObject("dotConnection", typeof(Image));
        // Asigna el padre del objeto.
        gameObjectConnection.transform.SetParent(_graphContainer, false);
        // Cambiar el color de la línea.
        gameObjectConnection.GetComponent<Image>().color = new Color(1,1,1, 1f);

        // Se configura el RectTransform de la línea y se dirige hacia el punto anterior con la longitud y ángulo adecuados.
        RectTransform rectTransformConnection = gameObjectConnection.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransformConnection.anchorMin = Vector2.zero;
        rectTransformConnection.anchorMax = Vector2.zero;
        rectTransformConnection.sizeDelta = new Vector2(distance, 0.5f);
        rectTransformConnection.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransformConnection.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    private float GetAngleFromVectorFloat(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    private int GetGraphValueAccordingTime(DateTime dateTime)
    {
        int sliderValue = (dateTime.Hour - _minHour) * 60 + dateTime.Minute;
        return sliderValue;
    }

    private void ModifyInitialHour(int hour)
    {
        _minHour = hour;
    }

    private void ModifyFinishHour(int hour)
    {
        _maxHour = hour;
    }
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


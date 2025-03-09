// Código basado en un tutorial de CodeMonkey
// Fuente: https://unitycodemonkey.com/video.php?v=CmU5-v-v1Qo
// Modificado para ajustarse a las necesidades del proyecto

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public enum GraphFilter
{
    Glycemia,
    Activity,
    Hunger
}

public class UI_Graph : MonoBehaviour
{
    [SerializeField] private Sprite _circleSprite;
    [SerializeField] private RectTransform _graphContainer;

    private int _minHour = 0;
    private int _maxHour = 0;
    private Dictionary<DateTime, int> _currentInfo = new Dictionary<DateTime, int>();
    private DateTime _currentDate = DateTime.Now;

    private GraphFilter _currentFilter;
    private GameObject _graphElements;
    private Color _lineColor;
    private List<GameObject> _x_labels;

    [Header("Prefabs")]
    [SerializeField] private GameObject _labelPrefab;
    [SerializeField] private GameObject _flashingLineXPrefab;
    [SerializeField] private GameObject _flashingLineYPrefab;

    private void Awake()
    {
        GameEventsGraph.OnInitialTimeModified += ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified += ModifyFinishHour;

        GameEventsGraph.OnUpdatedAttributeGraph += SetFilter;

        GameEventsGraph.OnUpdatedDateGraph += UpdateDate;
    }

    private void OnDestroy()
    {
        GameEventsGraph.OnInitialTimeModified -= ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified -= ModifyFinishHour;

        GameEventsGraph.OnUpdatedAttributeGraph -= SetFilter;

        GameEventsGraph.OnUpdatedDateGraph -= UpdateDate;
    }

    private void Start()
    {
        _x_labels = new List<GameObject>();

        // Creación del gameObject que contenga todos los elementos de la gráfica.
        _graphElements = new GameObject("Elements");
        _graphElements.transform.SetParent(_graphContainer, false);

        // Iniciar datos fecha actual.
        _currentFilter = GraphFilter.Glycemia;

        // Se establece el minimo y máximo de la franja horaria.
        ModifyInitialHour(LimitHours.Instance.initialTime.Hours);
        ModifyFinishHour(LimitHours.Instance.finishTime.Hours);
    }

    private void UpdateDate(DateTime newCurrentDate)
    {
        _currentDate = newCurrentDate;
        if (_graphElements.transform.childCount > 0)
        {
            foreach (Transform child in _graphElements.transform)
            {
                Destroy(child.gameObject);
            }
        }
        LoadData();
        ShowGraph();
        PlotDataPoints();
    }

    private void SetFilter(GraphFilter attribute)
    {
        _currentFilter = attribute;
        if (_graphElements.transform.childCount > 0)
        {
            foreach (Transform child in _graphElements.transform)
            {
                Destroy(child.gameObject);
            }
        }
        LoadData();
        ShowGraph();
        PlotDataPoints();
    }

    private void LoadData()
    {
        _currentInfo.Clear();

        switch (_currentFilter)
        {
            case GraphFilter.Glycemia:
                _currentInfo = DataStorage.LoadGlycemiaGraph(_currentDate);
                _lineColor = Color.red;
                break;
            case GraphFilter.Activity:
                _currentInfo = DataStorage.LoadActivityGraph(_currentDate);
                _lineColor = Color.yellow;
                break;
            case GraphFilter.Hunger:
                _currentInfo = DataStorage.LoadHungerGraph(_currentDate);
                _lineColor = Color.green;
                break;
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObjectCircle = new GameObject("circle", typeof(Image));
        // Asigna el padre del sprite.
        gameObjectCircle.transform.SetParent(_graphElements.transform, false);
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

    private void ShowGraph()
    {
        int maxHourAux = (_maxHour < _minHour) ? _maxHour + 24 : _maxHour;
        int numSeparations = maxHourAux + 1 - _minHour + 1;

        float graphHeight = _graphContainer.sizeDelta.y; // Tamaño máximo de altura.
        float graphWidth = _graphContainer.sizeDelta.x;// Tamaño máximo de ancho.

        float xSize = graphWidth / (numSeparations - 1); // Distancia horizontal entre punto y punto en el eje horizontal.

        // Configuración eje horizontal.
        for (int i = 0; i < numSeparations; i++)
        {
            float xPosition = i * xSize;

            // Colocación de la etiqueta de valor del eje horizontal.
            GameObject label_X_Object = Instantiate(_labelPrefab);
            RectTransform label_X = label_X_Object.GetComponent<RectTransform>();
            label_X.SetParent(_graphElements.transform, false);
            label_X.gameObject.SetActive(true);
            label_X.anchorMin = Vector2.zero;
            label_X.anchorMax = Vector2.zero;
            label_X.anchoredPosition = new Vector2(xPosition, -2.5f);
            if (i == numSeparations - 1)
            {
                int currentHour = (i + _minHour - 1) % 24;
                if (currentHour < 10)
                {
                    label_X.GetComponent<TextMeshProUGUI>().text = $"0{currentHour}:59";
                }
                else
                {
                    label_X.GetComponent<TextMeshProUGUI>().text = $"{currentHour}:59";
                }
            }
            else
            {
                int currentHour = (i + _minHour) % 24;
                if (currentHour < 10)
                {
                    label_X.GetComponent<TextMeshProUGUI>().text = $"0{currentHour}:00";
                }
                else
                {
                    label_X.GetComponent<TextMeshProUGUI>().text = $"{currentHour}:00";
                }
            }
            _x_labels.Add(label_X_Object);

            // Colocación de líneas intermitentes del eje horizontal.
            RectTransform flashingLine_X = Instantiate(_flashingLineXPrefab).GetComponent<RectTransform>();
            flashingLine_X.SetParent(_graphElements.transform, false);
            flashingLine_X.SetSiblingIndex(1);
            flashingLine_X.gameObject.SetActive(true);
            flashingLine_X.anchorMin = Vector2.zero;
            flashingLine_X.anchorMax = Vector2.zero;
            flashingLine_X.anchoredPosition = new Vector2(xPosition, -1.1f);
        }

        // Configuración eje vertical
        float yMaximum = _currentFilter == GraphFilter.Glycemia ? 250f : 100f; // Valor máximo en el eje vertical.
        int separatorCount = 10; // Cantidad de valores que se van a representar en el eje.
        for (int i = 0; i <= separatorCount; i++)
        {
            // Colocación de la etiqueta de valor del eje vertical.
            RectTransform label_Y = Instantiate(_labelPrefab).GetComponent<RectTransform>();
            label_Y.SetParent(_graphElements.transform, false);
            label_Y.gameObject.SetActive(true);
            label_Y.anchorMin = Vector2.zero;
            label_Y.anchorMax = Vector2.zero;
            float normalizedValue = i * 1f / separatorCount;
            label_Y.anchoredPosition = new Vector2(-5.5f, normalizedValue * graphHeight);
            label_Y.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();

            // Colocación de líneas intermitentes del eje vertical
            RectTransform flashingLine_Y = Instantiate(_flashingLineYPrefab).GetComponent<RectTransform>();
            flashingLine_Y.SetParent(_graphElements.transform, false);
            flashingLine_Y.SetSiblingIndex(1);
            flashingLine_Y.gameObject.SetActive(true);
            flashingLine_Y.anchorMin = Vector2.zero;
            flashingLine_Y.anchorMax = Vector2.zero;
            flashingLine_Y.anchoredPosition = new Vector2(-2.1f, normalizedValue * graphHeight);
        }

        SetFontSize();
    }

    private void PlotDataPoints()
    {
        float graphHeight = _graphContainer.sizeDelta.y;
        float graphWidth = _graphContainer.sizeDelta.x;
        float yMaximum = _currentFilter == GraphFilter.Glycemia ? 250f : 100f;

        // Ajustar el rango de horas si cruza la medianoche.
        int maxHourAux = (_maxHour < _minHour) ? _maxHour + 24 : _maxHour;
        int numSeparations = maxHourAux + 1 - _minHour + 1;

        GameObject lastGameObjectCircle = null;

        foreach (var dataPoint in _currentInfo)
        {
            // Hora y minutos del punto actual.
            DateTime time = dataPoint.Key;
            int currentHour = time.Hour;
            int currentMinute = time.Minute;

            // Ajuste de horas si el rango cruza la medianoche.
            if (currentHour < _minHour)
            {
                currentHour += 24;
            }

            // Se calcula el tiempo en formato decimal
            int limitMinutes = (currentHour == LimitHours.Instance.finishTime.Hours) ? 59 : 60;
            float currentTimeDecimal = currentHour + (currentMinute / (float)limitMinutes);

            // Se calcula la posición en el eje horizontal basada en el tiempo decimal.
            float xPosition = ((currentTimeDecimal - _minHour) / (numSeparations - 1)) * graphWidth;

            // Se calcula la posición en el eje vertical basada en el valor.
            float yPosition = (dataPoint.Value / yMaximum) * graphHeight;

            // Se calcula el círculo anterior con el nuevo
            GameObject currentGameObjectCircle = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastGameObjectCircle != null)
            {
                CreateDotConection(lastGameObjectCircle.GetComponent<RectTransform>().anchoredPosition, currentGameObjectCircle.GetComponent<RectTransform>().anchoredPosition);
            }
            lastGameObjectCircle = currentGameObjectCircle;
        }
    }

    private void CreateDotConection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObjectConnection = new GameObject("dotConnection", typeof(Image));
        // Asigna el padre del objeto.
        gameObjectConnection.transform.SetParent(_graphElements.transform, false);
        // Cambiar el color de la línea.
        Color lineColor = Color.white;

        gameObjectConnection.GetComponent<Image>().color = _lineColor;

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

    private void ModifyInitialHour(int hour)
    {
        _minHour = hour;
        UpdateDate(DateTime.Now);
    }

    private void ModifyFinishHour(int hour)
    {
        _maxHour = hour;
        UpdateDate(DateTime.Now);
    }

    private void SetFontSize()
    {
        float baseFontSize = 1.5f;
        float fontSizeAdjustment = 0.1f;
        float minFontSize = 0.95f;
        float maxFontSize = 1.5f;
        float maxHourDifference = 14;

        float maxHourAux = (_maxHour < _minHour) ? _maxHour + 24 : _maxHour;
        float hourDifference = maxHourAux - _minHour;

        if (hourDifference > maxHourDifference)
        {
            foreach (GameObject label in _x_labels)
            {
                float newFontSize = baseFontSize - (hourDifference - maxHourDifference) * fontSizeAdjustment;
                newFontSize = Mathf.Clamp(newFontSize, minFontSize, maxFontSize);

                label.GetComponent<TMP_Text>().fontSize = newFontSize;
            }
        }

        _x_labels.Clear();
    }
}
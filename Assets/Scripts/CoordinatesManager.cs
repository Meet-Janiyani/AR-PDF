using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;


public class CoordinatesManager : MonoBehaviour
{
    [Header("Custom Line-Renderer")]
    [SerializeField] GameObject bean;
    [SerializeField] GameObject cylinder;

    [Header("Parents for setting Instantiated Objects")]
    [SerializeField] Transform BeanParent;
    [SerializeField] Transform CoordinateParent;
    [SerializeField] Vector3 offset;
    


    [Header("Customization")]
    [SerializeField] float fromMin = 0;
    [SerializeField] float fromMax = 0;
    [SerializeField] float toMin = 0;
    [SerializeField] float toMax = 0;

    [Header("UI")]
    [SerializeField] TMP_Dropdown feature1;
    [SerializeField] TMP_Dropdown feature2;
    [SerializeField] TMP_Dropdown feature3;
    [SerializeField] GameObject coordinate;
    [SerializeField] GameObject X;
    [SerializeField] GameObject Y;
    [SerializeField] GameObject Z;

    [Header("Materials")]
    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;
    [SerializeField] Material greenMaterial;

    private List<GameObject> beans;
    private List<GameObject> cylinders;
    private List<GameObject> coordinates;
    public void Start()
    {
        beans= new List<GameObject>();
        cylinders = new List<GameObject>();
        coordinates = new List<GameObject>();   

        //StartCoroutine(Request());
    }

    IEnumerator Request()
    {
        string featur1_string = feature1.captionText.text;
        string featur2_string = feature2.captionText.text;
        string featur3_string = feature3.captionText.text;


        //string featur1_string = "x";
        //string featur2_string = "y";
        //string featur3_string = "z";

        APIRequest data = new APIRequest();
        data.items = new List<string>();
        data.items.Add(featur1_string);
        data.items.Add(featur2_string);
        data.items.Add((featur3_string));
        data.x_min = -5; data.x_max=5;

        
        string jsonData=JsonConvert.SerializeObject(data);
        byte[] bodyRaw=Encoding.UTF8.GetBytes(jsonData);

        // "https://ar-data-visualization-pi.vercel.app/lattice/"

        using (UnityWebRequest request= new UnityWebRequest("https://ar-data-visualization-pi.vercel.app/lattice/", "POST"))
        {
            request.uploadHandler=new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type","application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                string result_txt = request.downloadHandler.text;
                ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(result_txt);

                int n = response.number_of_features;

                //float[] x_coordinates=response.x_coordinates;
                //float[] y_coordinates=response.y_coordinates;
                //float[] z_coordinates=response.z_coordinates;

                float[] x_coordinates = RemapArray(response.x_coordinates, fromMin, fromMax, toMin, toMax);
                float[] y_coordinates = RemapArray(response.y_coordinates, fromMin, fromMax, toMin, toMax);
                float[] z_coordinates = RemapArray(response.z_coordinates, fromMin, fromMax, toMin, toMax);

                if (n == 1)
                {
                    for (int i = 0; i < x_coordinates.Length; i++)
                    {
                        GameObject newCylinder = Instantiate(cylinder, new Vector3(x_coordinates[i], 0, 0) / 10, bean.transform.rotation, BeanParent);
                        newCylinder.GetComponent<MeshRenderer>().material = redMaterial;
                    }
                    X.SetActive(true);
                    X.GetComponentInChildren<TMP_Text>().text = featur1_string;
                }
                else if (n == 2)
                {

                    //float y_min = Mathf.Abs( Mathf.Min(y_coordinates));

                    //for (int i = 0;i < x_coordinates.Length; i++)
                    //{
                    //    float x_i = x_coordinates[i];
                    //    for (int j = 0; j < y_coordinates.Length; j++)
                    //    {
                    //        float y_j = y_coordinates[j];
                    //        Vector3 spawnPosition=new Vector3(x_i,y_j,0) + new Vector3(0,y_min,0);
                    //        GameObject newBean = Instantiate(bean, spawnPosition, bean.transform.rotation, BeanParent);
                    //        beans.Add(newBean);
                    //    }
                    //}

                    float ymin = Mathf.Min(y_coordinates);
                    float ymax = Mathf.Max(y_coordinates);

                    float xmin = Mathf.Min(x_coordinates);
                    float xmax = Mathf.Max(x_coordinates);

                    Vector3 horizontalBegin = new Vector3(xmin, 0, 0);
                    Vector3 horizontalEnd = new Vector3(xmax, 0, 0);
                    Vector3 horizontalMid = (horizontalBegin + horizontalEnd) / 2f;
                    Vector3 horizontalDirection = horizontalEnd - horizontalBegin;
                    Quaternion horizontalRotation = Quaternion.FromToRotation(Vector3.up, horizontalDirection);
                    float horizontalScale = horizontalDirection.magnitude;
                    Vector3 horizontalScaleVector = new Vector3(cylinder.transform.localScale.x, horizontalScale / 2f, cylinder.transform.localScale.z);


                    Vector3 verticalBegin = new Vector3(0, ymin, 0);
                    Vector3 verticalEnd = new Vector3(0, ymax, 0);
                    Debug.Log(verticalEnd - verticalBegin);
                    Vector3 verticalMid = (verticalBegin + verticalEnd) / 2f;
                    Vector3 verticalDirection = verticalEnd - verticalBegin;
                    Quaternion verticalRotation = Quaternion.FromToRotation(Vector3.up, verticalDirection);
                    float verticalScale = verticalDirection.magnitude;
                    Vector3 verticalScaleVector = new Vector3(cylinder.transform.localScale.x, verticalScale / 2f, cylinder.transform.localScale.z);


                    for (int i = 0; i < y_coordinates.Length; i++)
                    {
                        horizontalMid.y = y_coordinates[i];

                        GameObject newCylinder = Instantiate(cylinder, BeanParent);
                        newCylinder.transform.position = horizontalMid;
                        newCylinder.transform.rotation = horizontalRotation;
                        newCylinder.transform.localScale = horizontalScaleVector;
                        newCylinder.GetComponent<MeshRenderer>().material = greenMaterial;
                        cylinders.Add(newCylinder);

                    }

                    for (int i = 0; i < x_coordinates.Length; i++)
                    {
                        verticalMid.x = x_coordinates[i];

                        GameObject newCylinder = Instantiate(cylinder,BeanParent);
                        newCylinder.transform.position = verticalMid;
                        newCylinder.transform.rotation = verticalRotation;
                        newCylinder.transform.localScale = verticalScaleVector;
                        newCylinder.GetComponent<MeshRenderer>().material = redMaterial;
                        cylinders.Add(newCylinder);
                    }

                    for (int i = 0; i < x_coordinates.Length; i++)
                    {
                        for (int j = 0; j < y_coordinates.Length; j++)
                        {
                            GameObject newCoordinate = Instantiate(coordinate, new Vector3(x_coordinates[i], y_coordinates[j]), coordinate.transform.rotation, CoordinateParent);
                            coordinates.Add(newCoordinate);
                        }
                    }
                    X.SetActive(true);
                    X.GetComponentInChildren<TMP_Text>().text = featur1_string;
                    Y.SetActive(true);
                    Y.GetComponentInChildren<TMP_Text>().text = featur2_string;



                }
                else if (n == 3)
                {
                    StartCoroutine(GenerateCylindersAndCoordinates(x_coordinates,y_coordinates,z_coordinates,featur1_string,featur2_string,featur3_string));
                }
                //debug_txt.text = "Completed";
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    public static float[] RemapArray(float[] values, float oldMin, float oldMax, float newMin, float newMax)
    {
        if (values == null)
        {
            return null;
        }

        float[] remapped = new float[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            remapped[i] = ((values[i] - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
        }

        return remapped;
    }

    public void DestroyBeans()
    {
        foreach(GameObject cylinder in cylinders)
        {
            Destroy(cylinder);
        }
        foreach(GameObject coordinate in coordinates)
        {
            Destroy(coordinate);
        }
        coordinates.Clear();
        beans.Clear();
    }

    public void OnSubmit()
    {
        StartCoroutine(Request());
    }

    IEnumerator GenerateCylindersAndCoordinates(float[] x_coordinates, float[] y_coordinates, float[] z_coordinates,string featur1_string, string featur2_string, string featur3_string)
    {
        // cache values to avoid repeated GetComponent or transform accesses
        float cylScaleX = cylinder.transform.localScale.x;
        float cylScaleZ = cylinder.transform.localScale.z;

        float xmin = Mathf.Min(x_coordinates);
        float xmax = Mathf.Max(x_coordinates);
        float ymin = Mathf.Min(y_coordinates);
        float ymax = Mathf.Max(y_coordinates);
        float zmin = Mathf.Min(z_coordinates);
        float zmax = Mathf.Max(z_coordinates);

        Vector3 horizontalScaleVector = new Vector3(cylScaleX, (xmax - xmin) / 2f, cylScaleZ);
        Vector3 verticalScaleVector = new Vector3(cylScaleX, (ymax - ymin) / 2f, cylScaleZ);
        Vector3 zScaleVector = new Vector3(cylScaleX, (zmax - zmin) / 2f, cylScaleZ);

        Quaternion horizontalRotation = Quaternion.FromToRotation(Vector3.up, new Vector3(xmax - xmin, 0, 0));
        Quaternion verticalRotation = Quaternion.FromToRotation(Vector3.up, new Vector3(0, ymax - ymin, 0));
        Quaternion zRotation = Quaternion.FromToRotation(Vector3.up, new Vector3(0, 0, zmax - zmin));

        int batchCounter = 0;
        int batchSize = 50;

        Vector3 position = Vector3.zero;

        float offsetx = Queryable.Average(x_coordinates.AsQueryable());
        float offsety = Queryable.Average(y_coordinates.AsQueryable());
        float offsetz = Queryable.Average(z_coordinates.AsQueryable());

        offset = new Vector3(offsetx,offsety,offsetz);
        // horizontal cylinders
        for (int i = 0; i < z_coordinates.Length; i++)
        {
            float zVal = z_coordinates[i];
            for (int j = 0; j < y_coordinates.Length; j++)
            {
                position.Set((xmin + xmax) / 2f, y_coordinates[j], zVal);
                var newCylinder = Instantiate(cylinder, position - offset, horizontalRotation, BeanParent);
                newCylinder.transform.localScale = horizontalScaleVector;
                newCylinder.GetComponent<MeshRenderer>().material = greenMaterial;
                cylinders.Add(newCylinder);

                if (++batchCounter >= batchSize)
                {
                    batchCounter = 0;
                    yield return null;
                }
            }
        }



        // vertical cylinders
        for (int i = 0; i < x_coordinates.Length; i++)
        {
            float xVal = x_coordinates[i];
            for (int j = 0; j < z_coordinates.Length; j++)
            {
                position.Set(xVal, (ymin + ymax) / 2f, z_coordinates[j]);
                var newCylinder = Instantiate(cylinder, position-offset, verticalRotation, BeanParent);
                newCylinder.transform.localScale = verticalScaleVector;
                newCylinder.GetComponent<MeshRenderer>().material = redMaterial;
                cylinders.Add(newCylinder);

                if (++batchCounter >= batchSize)
                {
                    batchCounter = 0;
                    yield return null;
                }
            }
        }

        // z-direction cylinders
        for (int i = 0; i < x_coordinates.Length; i++)
        {
            float xVal = x_coordinates[i];
            for (int j = 0; j < y_coordinates.Length; j++)
            {
                position.Set(xVal, y_coordinates[j], (zmin + zmax) / 2f);
                var newCylinder = Instantiate(cylinder, position-offset, zRotation, BeanParent);
                newCylinder.transform.localScale = zScaleVector;
                newCylinder.GetComponent<MeshRenderer>().material = blueMaterial;
                cylinders.Add(newCylinder);

                if (++batchCounter >= batchSize)
                {
                    batchCounter = 0;
                    yield return null;
                }
            }
        }

        // coordinate text objects
        for (int i = 0; i < x_coordinates.Length; i++)
        {
            float xVal = x_coordinates[i];
            for (int j = 0; j < y_coordinates.Length; j++)
            {
                float yVal = y_coordinates[j];
                for (int k = 0; k < z_coordinates.Length; k++)
                {
                    position.Set(xVal, yVal, z_coordinates[k]);
                    var newCoord = Instantiate(coordinate, position-offset+new Vector3(0,0.03f,0) , coordinate.transform.rotation, CoordinateParent);
                    newCoord.GetComponent<TMP_Text>().text = $"({xVal},{yVal},{z_coordinates[k]})";
                    coordinates.Add(newCoord);

                    if (++batchCounter >= batchSize)
                    {
                        batchCounter = 0;
                        yield return null;
                    }
                }
            }
        }

        // set axis labels
        X.SetActive(true);
        X.GetComponentInChildren<TMP_Text>().text = featur1_string;

        Y.SetActive(true);
        Y.GetComponentInChildren<TMP_Text>().text = featur2_string;

        Z.SetActive(true);
        Z.GetComponentInChildren<TMP_Text>().text = featur3_string;
    }


}

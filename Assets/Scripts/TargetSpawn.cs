using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] GameObject obstacle;
    public float time;
    public int randomIndex;
    public bool canSpanw;

    private GameObject[] targets;
    private GameObject[] spawnPoints;

    private void Awake()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawner");
        targets = new GameObject[spawnPoints.Length];
    }


    void Update()
    {
        //Condición para poder iniciar el spawneo
        switch (canSpanw)
        {
            case true:

                //Cuando puede, cuenta el tiempo
                time += Time.deltaTime;
                //Si el tiempo es igual o mayor a 2 segundos
                if (time >= 2f)
                {
                    //Generamos un número aleatorio entre 0 y la canditad de puntos que hemos hecho
                    randomIndex = Random.Range(0, spawnPoints.Length);
                    //Con esta función, aparecemos el objeto en la escena en el punto aleatorio que generamos antes
                    if (targets[randomIndex] == null)
                    {
                        targets[randomIndex] = Instantiate(obstacle, spawnPoints[randomIndex].transform.position, spawnPoints[randomIndex].transform.rotation);
                        targets[randomIndex].transform.SetParent(spawnPoints[randomIndex].transform);
                        //Devolvemos el contador a 0 para repetir el ciclo y limitar la cantidad de objetos que aparecen.
                        time = 0;
                    }
                    else
                        time = 1.5f;
                }

                break;

            case false:
                break;
        }
    }
}

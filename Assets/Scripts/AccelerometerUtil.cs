using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AccelerationBuffer{
  public float[] accelerations = new float[9];

  public AccelerationBuffer(){
    for(int i = 0; i < 9; i++) accelerations[i] = 0f;
  }

  public float nextFrame(float a){
    for(int i = 0; i < 8; i++) accelerations[i] = accelerations[i+1];
    accelerations[8] = a;
    return AVG();
  }

  private float AVG(){
    float res = 0f;
    for(int i = 0; i < 9; i++) res += accelerations[i];
    return res;
  }
}

public class AccelerometerUtil : MonoBehaviour{
    public float alpha = 0.8f;
    public float[] gravity = new float[3];

    public AccelerationBuffer xBuffer;
    public AccelerationBuffer zBuffer;
    Vector3 filteredValues = Vector3.zero;

    void Start(){

        Input.gyro.enabled = true;

        Vector3 currentAcc = Input.gyro.userAcceleration;
        gravity[0] = currentAcc.x;
        gravity[1] = currentAcc.y;
        gravity[2] = currentAcc.z;
        xBuffer = new AccelerationBuffer();
        zBuffer = new AccelerationBuffer();

        Debug.Log(xBuffer.nextFrame(Input.gyro.userAcceleration.x));
    }

    /*
    public Vector3 LowPassFiltered()
    {
      Vector3 filteredValues = Vector3.zero;

      Vector3 currentAcc = Input.gyro.userAcceleration;
      filteredValues[0] = currentAcc.x * alpha + filteredValues.x * (1f - alpha);
      filteredValues[1] = currentAcc.y * alpha + filteredValues.y * (1f - alpha);
      filteredValues[2] = currentAcc.z * alpha + filteredValues.z * (1f - alpha);

      return highPass(filteredValues);
    }
    */

    public Vector3 LowPassFiltered(){

      return new Vector3(xBuffer.nextFrame(Input.gyro.userAcceleration.x*Time.deltaTime), 0f,zBuffer.nextFrame(Input.gyro.userAcceleration.z*Time.deltaTime));
    }
    /*

    private float[] highPass(float x, float y, float z) {

float[] filteredValues = new float[3];

gravity[0] = ALPHA * gravity[0] + (1 – ALPHA) * x;
gravity[1] = ALPHA * gravity[1] + (1 – ALPHA) * y;
gravity[2] = ALPHA * gravity[2] + (1 – ALPHA) * z;

filteredValues[0] = x – gravity[0];
filteredValues[1] = y – gravity[1];
filteredValues[2] = z – gravity[2];

return filteredValues;

}

float[] lowPass(float x, float y, float z) {

float[] filteredValues = new float[3];

filteredValues[0] = x * a + filteredValues[0] * (1.0f – a);
filteredValues[1] = y * a + filteredValues[1] * (1.0f – a);
filteredValues[2] = z * a + filteredValues[2] * (1.0f – a);

return filteredValues;

}


    */
}

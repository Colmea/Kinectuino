#include <IRremote.h>
#include <Servo.h>

/* CONSTANTS
 * Edit values to your liking
 */ 
const int PIN_SERVO = 9; // Pin servo motor for direction
const int PIN_MOTOR = 5; // Pin DC Motor to move robot
const int PIN_MOTOR_FORWARD = 7; // Pin move robot forward
const int PIN_MOTOR_BACKWARD = 4; // Pin move robot backward
int angleMin = 85; // Min angle for servo motor (tend to 0)
int angleMax = 130; // Max angle for servor motor (tend to 180)
const int MAX = 1023;
const int HALF = 510;

/* Kinect vars
 * Calibrate these values ​​with the Kinect software provides
 */
const int valMaxSpeed = 620; // Max distance between your shoulder and your outstretched arm
const int valMinSpeed = 180; // Min distance between your shoulder and your outstretched arm
const int speedSwitchDirection = 400; // Arm distance (relative to your shoulder) when switch direction

// Vars (do not edit)
IRsend irsend;
Servo servo;
int angle;
int motorSpeed;
int motorDirection = 0;
int motorLastDirection;
int switchVal = 0;
String nodeData = "";
boolean dataComplete = false;
int potServoVal = 200;


void setup()
{
  
  // Moteur
  servo.attach(PIN_SERVO);

  pinMode(PIN_MOTOR, OUTPUT);
  pinMode(PIN_MOTOR_FORWARD, OUTPUT);
  pinMode(PIN_MOTOR_BACKWARD, OUTPUT);
  delay(5);
  
  pinMode(PIN_SERVO, OUTPUT);
  angle = 100;
  
  motorSpeed = 0;
  motorDirection = 1;
  motorLastDirection = 1;
  digitalWrite(PIN_MOTOR_FORWARD, HIGH);
  digitalWrite(PIN_MOTOR_BACKWARD, LOW);
  analogWrite(PIN_MOTOR, LOW);
  
  Serial.begin(9600);
  delay(100);

}

void loop()
{
  
  if (dataComplete)
  {   

      /* TV COMMANDS
       * feel free to add your own IR codes
       */
     
      // TV ON
      if (nodeData == "TVON")
      {
        // Samsung
        irsend.sendSamsung(0xE0E040BF, 32);
        
        // Sony
        irsend.sendSony(0xa90, 12);
        delay(100);
        irsend.sendSony(0xa90, 12);
        delay(100);
        irsend.sendSony(0xa90, 12);
        
        /* DEBUG
          Serial.print("tv on");
        */
      }
      
      // TV UP CHANNEL
      else if (nodeData == "TVUP")
      {
        // Samsung
        irsend.sendSamsung(0xE0E048B7, 32);
        
        /* DEBUG
          Serial.print("tv up channel");
        */
      }
      
      // TV DOWN CHANNEL
      else if (nodeData == "TVDOWN")
      {
        // Samsung
        irsend.sendSamsung(0xE0E008F7, 32);
        
        /* DEBUG
          Serial.print("tv down channel");
        */
      }
      
      // TV CHANNEL 3
      else if (nodeData == "TV3")
      {
        // Samsung
        irsend.sendSamsung(0xE0E0609F, 32);
        
        Serial.print("tv channel 3");
      }
      
      
      
      /* MOTION COMMANDS
       * To control speed and direction
       */
       
      // Motor speed command
      else if (nodeData.charAt(0) == 'M')
      {
        // If speed < speed switch direction, go backward
        if (nodeData.substring(1).toInt() < speedSwitchDirection)
        {
          motorDirection = -1;
          motorSpeed = map(nodeData.substring(1).toInt(), speedSwitchDirection, valMinSpeed, 0, 255);
        }
        // otherwise forward
        else
        {
          motorDirection = 1;
          motorSpeed = map(nodeData.substring(1).toInt(), speedSwitchDirection, valMaxSpeed, 0, 255);
        }
        
        /* DEBUG
          Serial.print("vitesse ");
          Serial.print(motorSpeed);
        */
      }
      
      // Steering command
      else if (nodeData.charAt(0) == 'D')
      {
        angle = map(nodeData.substring(1).toInt(), 0, 640, angleMin, angleMax);
      }
      
      else if (nodeData == "S")
      {
        digitalWrite(PIN_MOTOR, LOW);
      }
      
      else if (nodeData == "G")
      {
        angle = angleMin;
      }
      
      else if (nodeData == "D")
      {
        angle = angleMax;
      }
      
      // Set empty serial data
      dataComplete = false;
      nodeData = "";
  }
  
  
  // If switch direction
  if (motorDirection != motorLastDirection)
  {
    if (motorDirection == 1)
    {
      digitalWrite(PIN_MOTOR_FORWARD, LOW);
      digitalWrite(PIN_MOTOR_BACKWARD, LOW);
      delay(5);
      
      digitalWrite(PIN_MOTOR_FORWARD, HIGH);
      digitalWrite(PIN_MOTOR_BACKWARD, LOW);
      
      /* DEBUG
        Serial.print("forward");
      */
    }
    else
    {
      digitalWrite(PIN_MOTOR_FORWARD, LOW);
      digitalWrite(PIN_MOTOR_BACKWARD, LOW);
      delay(5);
      
      digitalWrite(PIN_MOTOR_BACKWARD, HIGH);
      digitalWrite(PIN_MOTOR_FORWARD, LOW);
      
      /* DEBUG
        Serial.print("backward");
      */
    }
    
    motorLastDirection = motorDirection;
  }
  
  // Speed motor
  analogWrite(PIN_MOTOR, motorSpeed);

  /* DEBUG MOTION
    Serial.print("Angle: ");
    Serial.print(motorDirection);
    Serial.print(" , speed: ");
    Serial.println(motorSpeed);
  */
  
  // prevent exceeding the angle
  if (angle < angleMin)
    angle = angleMin;
  if (angle > angleMax)
    angle = angleMax;
    
  servo.write(angle); 

}


// Function to retrieve serial data
void serialEvent()
{
  while (Serial.available())
   {
      char inChar = (char)Serial.read();
      
      if (inChar == '\n')
      {
        dataComplete = true;
      }
      else
      {
        nodeData += inChar;
      }
   }
}
  

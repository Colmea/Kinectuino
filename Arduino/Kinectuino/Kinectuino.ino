#include <Kinectuino.h>
#include <Servo.h>

/* 
 * CONSTANTS
 */ 
const int MAX = 1023;
const int HALF = 510;

Kinectuino kinectuino;
int led = 13; // led Arduino

void setup()
{
  pinMode(led, OUTPUT);
  kinectuino.init(); 
}

void loop()
{
  if (kinectuino.hasAvailableData())
  {
    digitalWrite(led, HIGH);
  }
  else
  {
    digitalWrite(led, LOW);
  }
}

// Push serial data to Kinectuino
void serialEvent()
{
  while (Serial.available()) {
    kinectuino.addSerialData((char)Serial.read());
  }
}


  

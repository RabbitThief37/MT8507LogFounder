/* -------------------------------------------------------------------------------
  Serial Event example
  When new serial data arrives, this sketch adds it to a String.
  When a newline is received, the loop prints the string and clears it.
  A good test for this is to try it with a GPS receiver that sends out
  NMEA 0183 sentences.
  NOTE: The serialEvent() feature is not available on the Leonardo, Micro, or
  other ATmega32U4 based boards.
  created 9 May 2011  by Tom Igoe
  This example code is in the public domain.
  http://www.arduino.cc/en/Tutorial/SerialEvent
  ---------------------------------------------------------------------------------- */
#include <IRremote.h>
#include <LiquidCrystal.h>

IRsend irsend;
LiquidCrystal lcd(8, 9, 4, 5, 6, 7);  // select the pins used on the LCD panel

String inputString = "";
bool stringComplete = false;

void setup() {
  lcd.begin(16, 2);              // start the library
  lcd.setCursor(0, 0);
  lcd.print("VIZIO SB REMOTE");

  Serial.begin(9600);
  inputString.reserve(16);
}

void loop() {
  if (stringComplete) {
    inputString = "";
    stringComplete = false;
  }
}

/* ----------------------------------------------------------------------------
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
  ------------------------------------------------------------------------------- */
void serialEvent() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();

    if (inChar == '\n') {
      SendIR();
      lcd.setCursor(0, 1);
      lcd.print("                ");
      delay(10);
      lcd.setCursor(0, 1);
      lcd.print(inputString);
      stringComplete = true;
    }
    else
    {
      // add it to the inputString:
      inputString += inChar;
    }
  }
}

/* -----------------------------------
  Send NEC remote control code to IR
  -------------------------------------- */
void SendIR() {
  if (inputString == "Power")
    irsend.sendNEC(0x00FF02FD, 32);
  else if (inputString == "ResetAll")
    irsend.sendNEC(0x00FFC639, 32);
  else if (inputString == "AUX")
    irsend.sendNEC(0x00FF8D72, 32);
  else if (inputString == "Optical")
    irsend.sendNEC(0x00FF13EC, 32);
  else if (inputString == "HDMI")
    irsend.sendNEC(0x00FFD52A, 32);
  else if (inputString == "HDMIARC")
    irsend.sendNEC(0x00FF59A6, 32);
  else if (inputString == "BT")
    irsend.sendNEC(0x00FF44BB, 32);
  else if (inputString == "USB")
    irsend.sendNEC(0x00FF857A, 32);
  else if (inputString == "VolUp")
    irsend.sendNEC(0x00FF827D, 32);
  else if (inputString == "VolDown")
    irsend.sendNEC(0x00FFA25D, 32);
  else if (inputString == "LeftCursor")
    irsend.sendNEC(0x00FF51AE, 32);
  else if (inputString == "RightCursor")
    irsend.sendNEC(0x00FFD12E, 32);
  else if (inputString == "BassDown")
    irsend.sendNEC(0x00FF56A9, 32);
  else if (inputString == "BassUp")
    irsend.sendNEC(0x00FFD629, 32);
  else if (inputString == "TrebleDown")
    irsend.sendNEC(0x00FF16E9, 32);
  else if (inputString == "TrebleUp")
    irsend.sendNEC(0x00FF9669, 32);
  else if (inputString == "CenterDown")
    irsend.sendNEC(0x00FF7689, 32);
  else if (inputString == "CenterUp")
    irsend.sendNEC(0x00FF0EF1, 32);
  else if (inputString == "HeightDown")
    irsend.sendNEC(0x00FF05FA, 32);
  else if (inputString == "HeightUp")
    irsend.sendNEC(0x00FF0DF2, 32);
  else if (inputString == "SurroundDown")
    irsend.sendNEC(0x00FF36C9, 32);
  else if (inputString == "SurroundUp")
    irsend.sendNEC(0x00FFB649, 32);
  else if (inputString == "BalanceDown")
    irsend.sendNEC(0x00FF2DD2, 32);
  else if (inputString == "BalanceUp")
    irsend.sendNEC(0x00FFCD32, 32);
  else if (inputString == "SubDown")
    irsend.sendNEC(0x00FF8A75, 32);
  else if (inputString == "SubUp")
    irsend.sendNEC(0x00FFB24D, 32);
  else if (inputString == "Movie")
    irsend.sendNEC(0x00FFE619, 32);
  else if (inputString == "Music")
    irsend.sendNEC(0x00FF6699, 32);
  else if (inputString == "Direct")
    irsend.sendNEC(0x00FF31CE, 32);
  else if (inputString == "HeightOn")
    irsend.sendNEC(0x00FFA15E, 32);
  else
    {
      lcd.setCursor(0, 1);
      lcd.print("ERROR           ");
    }
}

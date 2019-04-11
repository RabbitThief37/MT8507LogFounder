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
  if (inputString == "Power")				//VIZIO_RMC_CMD_POWER
    irsend.sendNEC(0x00FF02FD, 32);
  else if (inputString == "ResetAll")		//VIZIO_RMC_CMD_RESET_ALL
    irsend.sendNEC(0x00FFC639, 32);
  else if (inputString == "ResetAudio")		//VIZIO_RMC_CMD_RESET_AUDIO
    irsend.sendNEC(0x00FF01FE, 32);
  else if (inputString == "AUX")			//VIZIO_RMC_CMD_AUX
    irsend.sendNEC(0x00FF8D72, 32);
  else if (inputString == "Coxial")			//VIZIO_RMC_CMD_COXIAL
    irsend.sendNEC(0x00FF936C, 32);
  else if (inputString == "Optical")		//VIZIO_RMC_CMD_OPT
    irsend.sendNEC(0x00FF13EC, 32);
  else if (inputString == "HDMI")			//VIZIO_RMC_CMD_HDMI
    irsend.sendNEC(0x00FFD52A, 32);
  else if (inputString == "HDMIARC")		//VIZIO_RMC_CMD_ARC
    irsend.sendNEC(0x00FF59A6, 32);
  else if (inputString == "BT")				//VIZIO_RMC_CMD_BT
    irsend.sendNEC(0x00FF44BB, 32);
  else if (inputString == "USB")			//VIZIO_RMC_CMD_USB
    irsend.sendNEC(0x00FF857A, 32);
  else if (inputString == "VolUp")			//VIZIO_RMC_CMD_VOLUME_UP
    irsend.sendNEC(0x00FF827D, 32);
  else if (inputString == "VolDown")		//VIZIO_RMC_CMD_VOLUME_DOWN
    irsend.sendNEC(0x00FFA25D, 32);
  else if (inputString == "LeftCursor")		//VIZIO_RMC_CMD_LEFT_CURSOR
    irsend.sendNEC(0x00FF51AE, 32);
  else if (inputString == "RightCursor")	//VIZIO_RMC_CMD_RIGHT_CURSOR
    irsend.sendNEC(0x00FFD12E, 32);
  else if (inputString == "BassDown")		//VIZIO_RMC_CMD_BASS_DOWN
    irsend.sendNEC(0x00FF56A9, 32);
  else if (inputString == "BassUp")			//VIZIO_RMC_CMD_BASS_UP
    irsend.sendNEC(0x00FFD629, 32);
  else if (inputString == "TrebleDown")		//VIZIO_RMC_CMD_TREBLE_DOWN
    irsend.sendNEC(0x00FF16E9, 32);
  else if (inputString == "TrebleUp")		//VIZIO_RMC_CMD_TREBLE_UP
    irsend.sendNEC(0x00FF9669, 32);
  else if (inputString == "CenterDown")		//VIZIO_RMC_CMD_CENTER_DOWN
    irsend.sendNEC(0x00FF7689, 32);
  else if (inputString == "CenterUp")		//VIZIO_RMC_CMD_CENTER_UP
    irsend.sendNEC(0x00FF0EF1, 32);
  else if (inputString == "HeightDown")		//VIZIO_RMC_CMD_HEIGHT_LEVEL_DOWN
    irsend.sendNEC(0x00FF05FA, 32);
  else if (inputString == "HeightUp")		//VIZIO_RMC_CMD_HEIGHT_LEVEL_UP
    irsend.sendNEC(0x00FF0DF2, 32);
  else if (inputString == "SurroundDown")	//VIZIO_RMC_CMD_SURROUND_DOWN
    irsend.sendNEC(0x00FF36C9, 32);
  else if (inputString == "SurroundUp")		//VIZIO_RMC_CMD_SURROUND_UP
    irsend.sendNEC(0x00FFB649, 32);
  else if (inputString == "BalanceDown")	//VIZIO_RMC_CMD_BALANCE_DOWN
    irsend.sendNEC(0x00FF2DD2, 32);
  else if (inputString == "BalanceUp")		//VIZIO_RMC_CMD_BALANCE_UP
    irsend.sendNEC(0x00FFCD32, 32);
  else if (inputString == "SubDown")		//VIZIO_RMC_CMD_SUBWOOFER_DOWN
    irsend.sendNEC(0x00FF8A75, 32);
  else if (inputString == "SubUp")			//VIZIO_RMC_CMD_SUBWOOFER_UP
    irsend.sendNEC(0x00FFB24D, 32);
  else if (inputString == "Movie")			//VIZIO_RMC_CMD_EQ_MOVIE
    irsend.sendNEC(0x00FFE619, 32);
  else if (inputString == "Music")			//VIZIO_RMC_CMD_EQ_MUSIC
    irsend.sendNEC(0x00FF6699, 32);
  else if (inputString == "Direct")			//VIZIO_RMC_CMD_EQ_DIRECT
    irsend.sendNEC(0x00FF31CE, 32);
  else if (inputString == "HeightOn")		//VIZIO_RMC_CMD_HEIGHT_ON
    irsend.sendNEC(0x00FFA15E, 32);
  else if (inputString == "HeightOff")		//VIZIO_RMC_CMD_HEIGHT_OFF
    irsend.sendNEC(0x00FFC53A, 32);
  else if (inputString == "HeightVrt")		//VIZIO_RMC_CMD_HEIGHT_VRT
    irsend.sendNEC(0x00FFB14E, 32);
  else if (inputString == "Mute")			//VIZIO_RMC_CMD_MUTE
    irsend.sendNEC(0x00FF12ED, 32);
  else if (inputString == "BTPair")			//VIZIO_RMC_CMD_BT_PAIR
    irsend.sendNEC(0x00FFF609, 32);
  else if (inputString == "SurroundOn")		//VIZIO_RMC_CMD_SURROUND_ON
    irsend.sendNEC(0x00FFC43B, 32);
  else if (inputString == "SurroundOff")	//VIZIO_RMC_CMD_SURROUND_OFF
    irsend.sendNEC(0x00FF24DB, 32);
  else if (inputString == "VolLvOn")		//VIZIO_RMC_CMD_VOLUME_LEVELER_ON
    irsend.sendNEC(0x00FF14EB, 32);
  else if (inputString == "VolLvOff")		//VIZIO_RMC_CMD_VOLUME_LEVELER_OFF
    irsend.sendNEC(0x00FF42BD, 32);
  else if (inputString == "Info")			//VIZIO_RMC_CMD_INFO
    irsend.sendNEC(0x00FFD827, 32);
  else if (inputString == "NightOn")		//VIZIO_RMC_CMD_NIGHT_ON
    irsend.sendNEC(0x00FF8679, 32);
  else if (inputString == "NightOff")		//VIZIO_RMC_CMD_NIGHT_OFF
    irsend.sendNEC(0x00FF46B9, 32);
  else if (inputString == "DelayUp")		//VIZIO_RMC_CMD_AV_DELAY_UP
    irsend.sendNEC(0x00FF6B94, 32);
  else if (inputString == "DelayDown")		//VIZIO_RMC_CMD_AV_DELAY_DOWN
    irsend.sendNEC(0x00FF8E71, 32);
  else if (inputString == "SpkTest")		//VIZIO_RMC_CMD_SPEAKER_TEST
    irsend.sendNEC(0x00FF35CA, 32);
  else if (inputString == "VztvrmtOn")		//VIZIO_RMC_CMD_VZTVRMT_ON
    irsend.sendNEC(0x00FF7887, 32);
  else if (inputString == "VztvrmtOff")		//VIZIO_RMC_CMD_VZTVRMT_OFF
    irsend.sendNEC(0x00FFF807, 32);
  else if (inputString == "EcoOn")			//VIZIO_RMC_CMD_ECO_ON
    irsend.sendNEC(0x00FF649B, 32);
  else if (inputString == "EcoOff")			//VIZIO_RMC_CMD_ECO_OFF
    irsend.sendNEC(0x00FFE41B, 32);
  else if (inputString == "PwrLedOn")		//VIZIO_RMC_CMD_POWER_LED_ON
    irsend.sendNEC(0x00FF04FB, 32);
  else if (inputString == "PwrLedOff")		//VIZIO_RMC_CMD_POWER_LED_OFF
    irsend.sendNEC(0x00FF847B, 32);
  else if (inputString == "LearnVolUp")		//VIZIO_RMC_CMD_LEARN_VOLUME_UP
    irsend.sendNEC(0x00FFEE11, 32);
  else if (inputString == "LearnVolDown")	//VIZIO_RMC_CMD_LEARN_VOLUME_DOWN
    irsend.sendNEC(0x00FF906F, 32);
  else if (inputString == "LearnMute")		//VIZIO_RMC_CMD_LEARN_MUTE
    irsend.sendNEC(0x00FF4EB1, 32);
  else if (inputString == "WifiSetup")		//VIZIO_RMC_CMD_WIFI_SETUP
    irsend.sendNEC(0x00FFE11E, 32);
  else
    {
      lcd.setCursor(0, 1);
      lcd.print("ERROR           ");
    }
}

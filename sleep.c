#include <stdio.h>
#include <unistd.h>
#include "platform.h"
#include "xparameters.h"
#include "xscutimer.h"
#include "xscugic.h"
#include "xil_exception.h"
#include "xil_printf.h"
#include "xgpio.h"

#define SLEEP_SIGNAL_ID   XPAR_GPIO_0_DEVICE_ID
#define TIMER_DEVICE_ID   XPAR_XSCUTIMER_0_DEVICE_ID

XScuTimer TimerInstance;
XGpio SleepSignal;

int main() {

	init_platform();
	int Status;
	unsigned int CntValue1 = 0xFFFFFFFF, CntValue2 = 0, sleepTime = 0, time = 0;
	XScuTimer_Config *ConfigPtr;

	XGpio_Initialize(&SleepSignal, SLEEP_SIGNAL_ID);
	
	ConfigPtr = XScuTimer_LookupConfig(TIMER_DEVICE_ID);
	Status = XScuTimer_CfgInitialize(&TimerInstance, ConfigPtr,
																	 ConfigPtr->BaseAddr);
	if (Status != XST_SUCCESS) {
		return XST_FAILURE;
	}
	Status = XScuTimer_SelfTest(&TimerInstance);
	if (Status != XST_SUCCESS) {
		return XST_FAILURE;
	}

	XGpio_DiscreteWrite(&SleepSignal, 1, 0);

	while(1) {

		scanf("%u", &sleepTime);
		if (sleepTime != 0){

			sleep(sleepTime);
			
			XScuTimer_LoadTimer(&TimerInstance, CntValue1);
			XScuTimer_Start(&TimerInstance);
			XGpio_DiscreteWrite(&SleepSignal, 1, 1);
	
			sleep(sleepTime);

			CntValue2 = XScuTimer_GetCounterValue(&TimerInstance);
			XScuTimer_Stop(&TimerInstance);
			XGpio_DiscreteWrite(&SleepSignal, 1, 0);

			sleep(sleepTime*5);
			/* usleep(sleepTime); */
			
			time = CntValue1 - CntValue2;

			printf("%u\n", time);

			sleepTime = 0;						/* prevents double readings */
		}
		
	}
	cleanup_platform();
	return 0;
}

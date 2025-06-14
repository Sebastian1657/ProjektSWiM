/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * Copyright (c) 2025 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  ******************************************************************************
  */
  /* USER CODE END Header */
  /* Includes ------------------------------------------------------------------*/
#include "main.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */
typedef enum
{
	MOTOR_MODE_STOP = 0,
	MOTOR_MODE_FORWARD = 1,
	MOTOR_MODE_BACKWARD = -1
} MotorMode;

typedef enum
{
	TRACKS_LEFT,
	TRACKS_RIGHT
} TracksSide;
/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */

/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
TIM_HandleTypeDef htim3;

/* USER CODE BEGIN PV */

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_TIM3_Init(void);

/* USER CODE BEGIN PFP */
void drive(TracksSide tracksSide, MotorMode motorMode, int power);
void drive_left(MotorMode motorMode, int power);
void drive_right(MotorMode motorMode, int power);

void milestone_1_movement(void);
/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */
void drive(TracksSide tracksSide, MotorMode motorMode, int power)
{
	int offset = 0;
	if (power != 0) offset = 250;
	else offset = 0;

	GPIO_TypeDef* port1, port2;
	uint16_t pin1, pin2;
	int channel;

	switch (tracksSide)
	{
		case TRACKS_LEFT:
		{
			port1 = IN1_GPIO_Port;
			port2 = IN2_GPIO_Port;
			pin1 = IN1_Pin;
			pin2 = IN2_Pin;
			channel = TIM_CHANNEL_1;
			break;
		}
		case TRACKS_RIGHT:
		{
			port1 = IN3_GPIO_Port;
			port2 = IN4_GPIO_Port;
			pin1 = IN3_Pin;
			pin2 = IN4_Pin;
			channel = TIM_CHANNEL_2;
			break;
		}
	}

	switch (motorMode)
	{
		case MOTOR_MODE_FORWARD:
		{
			HAL_GPIO_WritePin(port1, pin1, 1);
			HAL_GPIO_WritePin(port2, pin2, 0);
			break;
		}
		case MOTOR_MODE_BACKWARD:
		{
			HAL_GPIO_WritePin(port1, pin1, 0);
			HAL_GPIO_WritePin(port2, pin2, 1);
			break;
		}
		case MOTOR_MODE_STOP:
		{
			HAL_GPIO_WritePin(port1, pin1, 0);
			HAL_GPIO_WritePin(port2, pin2, 0);
			break;
		}
	}
	__HAL_TIM_SET_COMPARE(&htim3, channel, power + offset);
}

void drive_left(MotorMode motorMode, int power)
{
	drive(TRACKS_LEFT, motorMode, power);
}

void drive_right(MotorMode motorMode, int power)
{
	drive(TRACKS_RIGHT, motorMode, power);
}

void milestone_1_movement()
{
	//rozkaz 1 - jazda na wprost, ze stałą prędkością
	drive_left(MOTOR_MODE_FORWARD, 250);
	drive_right(MOTOR_MODE_FORWARD, 250);
	HAL_Delay(2000);

	//rozkaz 2 - jazda na wprost zygzakiem
	drive_left(MOTOR_MODE_FORWARD, 150);
	drive_right(MOTOR_MODE_FORWARD, 350);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_FORWARD, 350);
	drive_right(MOTOR_MODE_FORWARD, 150);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_FORWARD, 150);
	drive_right(MOTOR_MODE_FORWARD, 350);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_FORWARD, 350);
	drive_right(MOTOR_MODE_FORWARD, 150);
	HAL_Delay(500);

	//rozkaz 3 - zatrzymanie pojazdu
	drive_left(MOTOR_MODE_STOP, 0);
	drive_right(MOTOR_MODE_STOP, 0);
	HAL_Delay(2000);

	//rozkaz 4 - jazda w kółko w lewo
	drive_left(MOTOR_MODE_FORWARD, 200);
	drive_right(MOTOR_MODE_STOP, 0);
	HAL_Delay(3000);

	//rozkaz 5 - jazda w kółko w prawo
	drive_left(MOTOR_MODE_STOP, 0);
	drive_right(MOTOR_MODE_FORWARD, 200);
	HAL_Delay(3000);

	//rozkaz 6 - jazda do tyłu
	drive_left(MOTOR_MODE_BACKWARD, 250);
	drive_right(MOTOR_MODE_BACKWARD, 250);
	HAL_Delay(2000);

	//rozkaz 7 - jazda do tyłu zygzakiem
	drive_left(MOTOR_MODE_BACKWARD, 150);
	drive_right(MOTOR_MODE_BACKWARD, 350);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_BACKWARD, 350);
	drive_right(MOTOR_MODE_BACKWARD, 150);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_BACKWARD, 150);
	drive_right(MOTOR_MODE_BACKWARD, 350);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_BACKWARD, 350);
	drive_right(MOTOR_MODE_BACKWARD, 150);
	HAL_Delay(500);

	//rozkaz 8 - jazda do przodu, ze zmienną prędkością
	for (int i = 0; i < 400; i += 10)
	{
		drive_left(MOTOR_MODE_FORWARD, i);
		drive_right(MOTOR_MODE_FORWARD, i);
		HAL_Delay(100);
	}

	//rozkaz 9 - jazda do tyłu, ze zmienną prędkością
	for (int i = 0; i < 400; i += 10)
	{
		drive_left(MOTOR_MODE_BACKWARD, i);
		drive_right(MOTOR_MODE_BACKWARD, i);
		HAL_Delay(100);
	}

	//rozkaz 10 - wykręcanie
	drive_left(MOTOR_MODE_FORWARD, 350);
	drive_right(MOTOR_MODE_FORWARD, 150);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_BACKWARD, 150);
	drive_right(MOTOR_MODE_BACKWARD, 350);
	HAL_Delay(500);

	drive_left(MOTOR_MODE_FORWARD, 400);
	drive_right(MOTOR_MODE_FORWARD, 400);
	HAL_Delay(1000);
}

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{

	/* USER CODE BEGIN 1 */


	/* USER CODE END 1 */

	/* MCU Configuration--------------------------------------------------------*/

	/* Reset of all peripherals, Initializes the Flash interface and the Systick. */
	HAL_Init();

	/* USER CODE BEGIN Init */

	/* USER CODE END Init */

	/* Configure the system clock */
	SystemClock_Config();

	/* USER CODE BEGIN SysInit */

	/* USER CODE END SysInit */

	/* Initialize all configured peripherals */
	MX_GPIO_Init();
	MX_TIM3_Init();
	/* USER CODE BEGIN 2 */
	HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
	HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_2);

	/* USER CODE END 2 */

	/* Infinite loop */
	/* USER CODE BEGIN WHILE */
	while (1)
	{
		HAL_Delay(3000);
		milestone_1_movement();
		/* USER CODE END WHILE */

		/* USER CODE BEGIN 3 */
	}
	/* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
	RCC_OscInitTypeDef RCC_OscInitStruct = { 0 };
	RCC_ClkInitTypeDef RCC_ClkInitStruct = { 0 };
	RCC_PeriphCLKInitTypeDef PeriphClkInit = { 0 };

	/** Initializes the RCC Oscillators according to the specified parameters
	* in the RCC_OscInitTypeDef structure.
	*/
	RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
	RCC_OscInitStruct.HSIState = RCC_HSI_ON;
	RCC_OscInitStruct.HSICalibrationValue = RCC_HSICALIBRATION_DEFAULT;
	RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
	RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSI;
	RCC_OscInitStruct.PLL.PLLMUL = RCC_PLL_MUL8;
	RCC_OscInitStruct.PLL.PREDIV = RCC_PREDIV_DIV1;
	if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
	{
		Error_Handler();
	}

	/** Initializes the CPU, AHB and APB buses clocks
	*/
	RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK | RCC_CLOCKTYPE_SYSCLK
		| RCC_CLOCKTYPE_PCLK1 | RCC_CLOCKTYPE_PCLK2;
	RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
	RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
	RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV2;
	RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

	if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_2) != HAL_OK)
	{
		Error_Handler();
	}
	PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_TIM34;
	PeriphClkInit.Tim34ClockSelection = RCC_TIM34CLK_HCLK;
	if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
	{
		Error_Handler();
	}
}

/**
  * @brief TIM3 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM3_Init(void)
{

	/* USER CODE BEGIN TIM3_Init 0 */

	/* USER CODE END TIM3_Init 0 */

	TIM_ClockConfigTypeDef sClockSourceConfig = { 0 };
	TIM_MasterConfigTypeDef sMasterConfig = { 0 };
	TIM_OC_InitTypeDef sConfigOC = { 0 };

	/* USER CODE BEGIN TIM3_Init 1 */

	/* USER CODE END TIM3_Init 1 */
	htim3.Instance = TIM3;
	htim3.Init.Prescaler = 15;
	htim3.Init.CounterMode = TIM_COUNTERMODE_UP;
	htim3.Init.Period = 200;
	htim3.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
	htim3.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_ENABLE;
	if (HAL_TIM_Base_Init(&htim3) != HAL_OK)
	{
		Error_Handler();
	}
	sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
	if (HAL_TIM_ConfigClockSource(&htim3, &sClockSourceConfig) != HAL_OK)
	{
		Error_Handler();
	}
	if (HAL_TIM_PWM_Init(&htim3) != HAL_OK)
	{
		Error_Handler();
	}
	sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
	sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
	if (HAL_TIMEx_MasterConfigSynchronization(&htim3, &sMasterConfig) != HAL_OK)
	{
		Error_Handler();
	}
	sConfigOC.OCMode = TIM_OCMODE_PWM1;
	sConfigOC.Pulse = 0;
	sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
	sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
	if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_1) != HAL_OK)
	{
		Error_Handler();
	}
	if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_2) != HAL_OK)
	{
		Error_Handler();
	}
	/* USER CODE BEGIN TIM3_Init 2 */

	/* USER CODE END TIM3_Init 2 */
	HAL_TIM_MspPostInit(&htim3);

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
	GPIO_InitTypeDef GPIO_InitStruct = { 0 };
	/* USER CODE BEGIN MX_GPIO_Init_1 */

	/* USER CODE END MX_GPIO_Init_1 */

	/* GPIO Ports Clock Enable */
	__HAL_RCC_GPIOC_CLK_ENABLE();
	__HAL_RCC_GPIOA_CLK_ENABLE();
	__HAL_RCC_GPIOB_CLK_ENABLE();

	/*Configure GPIO pin Output Level */
	HAL_GPIO_WritePin(IN3_GPIO_Port, IN3_Pin, GPIO_PIN_RESET);

	/*Configure GPIO pin Output Level */
	HAL_GPIO_WritePin(GPIOB, IN4_Pin | IN1_Pin | IN2_Pin, GPIO_PIN_RESET);

	/*Configure GPIO pin : btn_Pin */
	GPIO_InitStruct.Pin = btn_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_IT_RISING;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	HAL_GPIO_Init(btn_GPIO_Port, &GPIO_InitStruct);

	/*Configure GPIO pin : IN3_Pin */
	GPIO_InitStruct.Pin = IN3_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
	HAL_GPIO_Init(IN3_GPIO_Port, &GPIO_InitStruct);

	/*Configure GPIO pins : IN4_Pin IN1_Pin IN2_Pin */
	GPIO_InitStruct.Pin = IN4_Pin | IN1_Pin | IN2_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
	GPIO_InitStruct.Pull = GPIO_NOPULL;
	GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
	HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

	/* USER CODE BEGIN MX_GPIO_Init_2 */

	/* USER CODE END MX_GPIO_Init_2 */
}

/* USER CODE BEGIN 4 */

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
	/* USER CODE BEGIN Error_Handler_Debug */
	/* User can add his own implementation to report the HAL error return state */
	__disable_irq();
	while (1)
	{
	}
	/* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t* file, uint32_t line)
{
	/* USER CODE BEGIN 6 */
	/* User can add his own implementation to report the file name and line number,
	   ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
	   /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT *//* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * Copyright (c) 2025 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  ******************************************************************************
  */
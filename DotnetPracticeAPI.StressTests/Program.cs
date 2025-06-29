using DotnetPracticeAPI.StressTests.Scenarios;

int userCount = 100;
int durationSec = 60;

UserServiceStressTest.Run(userCount, durationSec);
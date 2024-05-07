pipeline {
    agent any
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    }
    stages {
       
        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }
		stage('Build') {
            steps {
                sh 'dotnet build'
            }
        }
        
        stage('Publish') {
            steps {
			    sh 'sudo systemctl stop MerlinLauncher.service'
                sh 'dotnet publish -c Release -o /usr/local/merlin'
				sh 'sudo systemctl start MerlinLauncher.service'
            }
        }
    }
}

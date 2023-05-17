pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                // Checkout your code from GitHub
                git branch: 'master ', url: 'https://github.com/gouthamharidas/pushdemo.git'
            }
        }

        stage('Build') {
            steps {
                // Execute MSBuild command to build your C#.NET project
                sh 'msbuild PushListenerForLinux.sln /p:Configuration=Release'
            }
        }

        stage('SonarQube Analysis') {
            steps {
                // Run SonarQube scanner for code analysis
                withSonarQubeEnv('SonarQube') {
                    sh "dotnet sonarscanner begin /k:your-project-key /d:sonar.host.url=$http://172.16.15.41:9000 /d:sonar.login=$test1"
                    sh 'msbuild PushListenerForLinux.sln'
                    sh "dotnet sonarscanner end /d:sonar.login=$test1"
                }
            }
        }
    }

    post {
        always {
            // Publish SonarQube analysis results and wait for quality gate
            waitForQualityGate()
        }
    }
}

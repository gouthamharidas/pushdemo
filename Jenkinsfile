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
        // Change directory to the folder containing the solution file
        dir('/PushListenerForLinux') {
            // Execute MSBuild command to build your C#.NET project
            sh 'msbuild PushListenerForLinux.sln /p:Configuration=Release'
        }
    }
}


        stage('SonarQube Analysis') {
            steps {
                // Run SonarQube scanner for code analysis
                withSonarQubeEnv('SonarQube') {
                    sh "dotnet sonarscanner begin /k:test1 /d:sonar.host.url=http://172.16.15.41:9000 /d:sonar.login=77b9e400b7c50502c28b628a2b4798c722138946"
                    sh 'msbuild PushListenerForLinux.sln'
                    sh "dotnet sonarscanner end /d:sonar.login=$test1"
                }
            }
        }
    }

    
}

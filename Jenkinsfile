pipeline {
    agent any

    stages {
        stage('Version') {
            agent {
                docker { image 'node:14-alpine' }
            }
            steps {
                sh 'node --version'
            }
        }

        stage('Build') {
            steps {
                script {
                    def image = docker.build("project-ivy-api", "./src/ProjectIvy.Api/Dockerfile")
                }
            }
        }
    }
}
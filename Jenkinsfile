pipeline {
    agent any

    stages {
        stage('Version') {
            agent {
                docker { image 'node:14-alpine' }
            }
            steps {
                sh 'ls'
                sh 'node --version'
            }
        }

        stage('Build') {
            steps {
                script {
                    checkout scm
                    def image = docker.build("project-ivy-api", "src/ProjectIvy.Api/Dockerfile")
                }
            }
        }
    }
}
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
            agent any
            steps {
                script {
                    checkout scm
                    def image = docker.build("project-ivy-api")
                }
            }
        }
    }
}
pipeline {
    agent {
        label 'worker'
    }

    stages {
        stage('Version') {
            agent {
                label 'worker'
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
                    def image = docker.build("project-ivy-api")
                }
            }
        }
    }
}
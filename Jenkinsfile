pipeline {
    agent {
        docker { image 'node:14-alpine' }
    }

    stages {
        stage('Build') {
            node {
                steps {
                    sh 'node --version'
                }
            }
        }
    }
}
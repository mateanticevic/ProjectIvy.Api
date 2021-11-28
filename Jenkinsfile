pipeline {
    agent {
        label 'worker'
    }

    stages {
        // stage('Version') {
        //     agent {
        //         docker { image 'node:14-alpine' }
        //     }
        //     steps {
        //         sh 'ls'
        //         sh 'node --version'
        //     }
        // }

        stage('Build') {
            
            steps {
                script {
                    sh 'export PATH="$PATH:$HOME/.dotnet/tools/"'
                    sh 'echo $PATH'
                    def version = sh(script:'dotnet-gitversion /showvariable FullSemVer', returnStdout:true).trim()
                    currentBuild.displayName = version

                    def image = docker.build("project-ivy-api")
                    image.push(version)
                    image.push()
                }
            }
        }
    }
}
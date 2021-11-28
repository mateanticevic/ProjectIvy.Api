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
                   def semver = sh(script:'/home/mate/.dotnet/tools/dotnet-gitversion /showvariable SemVer', returnStdout:true).trim()
                   def commits = sh(script:'/home/mate/.dotnet/tools/dotnet-gitversion /showvariable CommitsSinceVersionSource', returnStdout:true).trim()
                   def version = "${semver}.${commits}"

                   currentBuild.displayName = version

                   def image = docker.build("mateanticevic/project-ivy-api")
                   image.push(version)
                   image.push()
                }
            }
        }
    }
}
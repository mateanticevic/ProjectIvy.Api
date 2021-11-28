pipeline {
    agent {
        label 'worker'
    }
    stages {
        stage('Build') {
            steps {
                script {
                   def semver = sh(script:'/home/mate/.dotnet/tools/dotnet-gitversion /showvariable SemVer', returnStdout:true).trim()
                   def commits = sh(script:'/home/mate/.dotnet/tools/dotnet-gitversion /showvariable CommitsSinceVersionSource', returnStdout:true).trim()
                   def version = "${semver}"

                   if (commits != '0' && commits != 0) {
                       version = "${version}-${commits}"
                   }

                   currentBuild.displayName = version

                   def image = docker.build("mateanticevic/project-ivy-api")

                   if (version == semver) {
                       image.push(version)
                       image.push()
                   }
                }
            }
        }
    }
}
SOLUTION    := RomForge.slnx
UI_PROJECT  := src/RomForge.UI/RomForge.UI.csproj
APP_BUNDLE  := artifacts/RomForge.app
PUBLISH_DIR := artifacts/publish/osx-arm64

.PHONY: build test run check clean coverage package sonar sonar-start

build:
	dotnet build $(SOLUTION) --verbosity quiet

test:
	dotnet test $(SOLUTION) --verbosity quiet

run:
	dotnet run --project $(UI_PROJECT)

check:
	dotnet clean $(SOLUTION) --verbosity quiet
	dotnet build $(SOLUTION) --verbosity quiet
	dotnet test $(SOLUTION) --verbosity quiet

coverage:
	dotnet test $(SOLUTION) --collect:"XPlat Code Coverage" --settings coverage.runsettings --results-directory artifacts/coverage --verbosity quiet

clean:
	dotnet clean $(SOLUTION) --verbosity quiet

package:
	dotnet publish $(UI_PROJECT) -c Release -r osx-arm64 --self-contained true -o $(PUBLISH_DIR)

# One-time setup:
#   brew install sonar-scanner
#   docker run -d --name sonarqube -p 9000:9000 sonarqube:lts-community
#   Log in at http://localhost:9000 (admin / admin on first run)
#   My Account → Security → Generate Token → export SONAR_TOKEN=<token>
sonar-start:
	docker run -d --name sonarqube -p 9000:9000 sonarqube:lts-community

sonar: coverage
	sonar-scanner

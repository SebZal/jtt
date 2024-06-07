build:
	dotnet publish --property:PublishDir=../publish -c Release

install: build
	cp ./publish/jtt /usr/local/bin

uninstall:
	rm /usr/local/bin/jtt

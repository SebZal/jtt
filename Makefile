build:
	dotnet publish --property:PublishDir=../publish -c Release

install: build
	cp -r ./publish/jtt /usr/local/bin

uninstall:
	rm -rf /usr/local/bin/jtt

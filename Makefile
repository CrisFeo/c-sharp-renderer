MAIN_PROJECT='renderer'

.PHONY: clean
clean:
	cd $(MAIN_PROJECT) && make $<

.PHONY: build
build:
	cd $(MAIN_PROJECT) && make $<

.PHONY: build-release
build-release:
	cd $(MAIN_PROJECT) && make $<

.PHONY: run
run:
	cd $(MAIN_PROJECT) && make $<

.PHONY: run-release
run-release:
	cd $(MAIN_PROJECT) && make $<

.PHONY: run-trace
run-trace:
	cd $(MAIN_PROJECT) && make $< < /dev/null > /dev/null &
	./scripts/trace
